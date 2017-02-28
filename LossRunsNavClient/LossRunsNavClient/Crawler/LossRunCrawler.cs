using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LossRunsNavClient.Crawler
{
    public static class LossRunCrawler
    {
        static string batchName;
        [DllImport("wininet.dll", CharSet=CharSet.Auto, SetLastError = true)]
        private static extern bool InternetGetCookieEx(
            string url,
            string cookieName,
            StringBuilder cookieData,
            ref int size,
            int flags,
            IntPtr pReserved);

        //static DebugUtils.ErrorLogger crawlLogger = new DebugUtils.ErrorLogger("Crawler", false, true);

        public static LossRunCrawlerResult Process(WebBrowser browser, string policy)
        {
            string batName = "batches";
            return Process(browser, policy, false, batName);
        }

        public static LossRunCrawlerResult Process(WebBrowser browser, string policy, string batName)
        {
            return Process(browser, policy, false, batName);
        }

        public static LossRunCrawlerResult Process(WebBrowser browser, string policy, bool runDuplicate, string batName)
        {
            return Process(browser, policy, runDuplicate, 0, batName);
        }

        public static LossRunCrawlerResult Process(WebBrowser browser, string policy, bool runDuplicate, int documentLevel, string batName)
        {
            batchName = batName;
            Debug.WriteLine("Started Web Crawler");
            AutoResetEvent monitor = new AutoResetEvent(false);
            CookieContainer cookieJar = new CookieContainer();
            browser.ClearEventHandlers("DocumentCompleted");
            browser.DocumentCompleted += (s, e) => monitor.Set();
            if (!runDuplicate)
            {
                if (documentLevel == 0)
                {
                    if (FindBatchFile(policy)) { return new LossRunCrawlerResult() { Result = LossRunResult.Duplicate }; }
                }
                else
                {
                    if (FindBatchFile(policy, documentLevel)) { return new LossRunCrawlerResult() { Result = LossRunResult.Duplicate }; }
                }
            }
            var connected = false;
            int attempts = 0;
            while (!connected)
            {
                try
                {
                    browser.Navigate(@"https://my.navg.com/LossRuns/Login.aspx");
                    connected = true;
                }
                catch
                {
                    connected = false;
                    //crawlLogger.AddLine("Failed to connect: " + attempts.ToString() + "Attempts thus far");
                    if (attempts % 5 == 0)
                    {
                        //crawlLogger.AddLine("Sleeping");
                        if (attempts > 5)
                            Thread.Sleep(5000 * 60);
                        if (attempts > 50)
                            Thread.Sleep(1000 * 60 * 80);
                    }
                    attempts++;
                }
            }
            browser.UntilLoaded(monitor);
         
            if (!browser.Document.ExpectLoginScreen()) { return new LossRunCrawlerResult() { Result = LossRunResult.Failure, Message = "Was not the login screen." }; }
            HtmlDocument loginScreen = browser.Document;
            if (
                !loginScreen.SetFieldInTemplate("lgnBroker_UserName", "BTIS")
                || !loginScreen.SetFieldInTemplate("lgnBroker_Password", "BTIS2015")
                || !loginScreen.ClickInTemplate("lgnBroker_LoginButton")
                )
            {
               return new LossRunCrawlerResult() { Result = LossRunResult.Failure, Message = "Could not login on login screen." };
            }
            browser.UntilLoaded(monitor);

            if (!browser.Document.ExpectSearchScreen()) { return new LossRunCrawlerResult() { Result = LossRunResult.Failure, Message = "Could not log in.  Was not the search screen." }; }
            HtmlDocument searchScreen = browser.Document;
            if (
                !searchScreen.SetFieldInTemplate("txtPolicyNumber", policy)
                || !searchScreen.ClickInTemplate("btnSearch")
               )
            {
                return new LossRunCrawlerResult() { Result = LossRunResult.Failure, Message = "Could not search on search screen." };
            }
            browser.UntilLoaded(monitor);
  

            IEnumerable<int> documentLevels = Enumerable.Empty<int>();
            if (browser.Document.ExpectMultipleResultsScreen())
            {
                IEnumerable<HtmlElement> elements = browser.Document.NavgSpecificSelectButtons().Skip(documentLevel).Take(1);
                if (!elements.Any())
                {
                    return new LossRunCrawlerResult() { Result = LossRunResult.Failure, Message = string.Format("Could not retrieve expected multiple results for {0} - {1}", policy, documentLevel) };
                }
                elements.Single().InvokeMember("click");

                browser.UntilLoaded(monitor);
            }
            else if (documentLevel > 0)
            {
                return new LossRunCrawlerResult() { Result = LossRunResult.Failure, Message = string.Format("Could not retrieve expected multiple results for {0} - {1}", policy, documentLevel) };
            }

            if (!browser.Document.ExpectSingleResultScreen()) { return new LossRunCrawlerResult() { Result = LossRunResult.Failure, Message = "No results on the result page." }; }
            HtmlDocument resultsScreen = browser.Document;

            //set login cookie
            int cookieSize = 1024;
            StringBuilder cookieDough = new StringBuilder(cookieSize);
            if (!InternetGetCookieEx("https://my.navg.com", null, cookieDough, ref cookieSize, 0x00002000, IntPtr.Zero))
            {
                if (cookieSize > 0)
                {
                    cookieDough = new StringBuilder(cookieSize);
                    InternetGetCookieEx("https://my.navg.com", null, cookieDough, ref cookieSize, 0x00002000, IntPtr.Zero);
                }
            }

            foreach (HtmlElement iframe in
                resultsScreen
                .ElementInTemplate("resultsIFrameId")
                .Where(iframe => !string.IsNullOrEmpty(iframe.GetAttribute("src")))
                )
            {
                string contentUrl = iframe.GetAttribute("src");
                string cannonicalUrl = 
                    contentUrl.StartsWith("http") ? contentUrl :
                    contentUrl.StartsWith("/") ? string.Format("https://my.navg.com{0}", contentUrl) :
                    string.Format("https://my.navg.com/LossRuns/{0}", contentUrl);
                HttpWebRequest resultRequest = WebRequest.CreateHttp(cannonicalUrl);
                resultRequest.CookieContainer = new CookieContainer();
                resultRequest.CookieContainer.SetCookies(new Uri("https://my.navg.com"), cookieDough.ToString());
                resultRequest.Referer = "https://my.navg.com/LossRuns/LossReportsSearch.aspx";
                resultRequest.KeepAlive = false;
                resultRequest.ProtocolVersion = HttpVersion.Version10;

                HttpWebResponse resultResponse = resultRequest.GetResponse() as HttpWebResponse;
                byte[] resultResponseContent;
                using(MemoryStream buffer = new MemoryStream())
                {
                    using(Stream responseStream = resultResponse.GetResponseStream())
                    {
                        byte[] intermediary = new byte[16384];
                        int bytesRead;
                        while (0 < (bytesRead = responseStream.Read(intermediary, 0, intermediary.Length)))
                        {
                            buffer.Write(intermediary, 0, bytesRead);
                        }
                        buffer.Flush();
                    }
                    buffer.Seek(0L, SeekOrigin.Begin);
                    resultResponseContent = buffer.ToArray();
                }
                //78e0 - 37 38 65 30
                //newline - 0d 0A
                if(
                    resultResponseContent.Length < 4
                ||  resultResponseContent[0] != 0x25
                ||  resultResponseContent[1] != 0x50
                ||  resultResponseContent[2] != 0x44
                ||  resultResponseContent[3] != 0x46
                    ) { return new LossRunCrawlerResult() { Result = LossRunResult.Failure, Message = "Not a pdf file." }; }
                SavePolicy(policy, documentLevel, resultResponseContent);
            }

            foreach (int otherLevel in documentLevels)
            {
                LossRunCrawlerResult otherResult = Process(browser, policy, runDuplicate, otherLevel, batchName);
                if (otherResult.Result == LossRunResult.Failure)
                {
                    return otherResult;
                }
            }
            Debug.WriteLine("Crawler Success");
            return new LossRunCrawlerResult() { Result = LossRunResult.Success };
        }


        static bool SavePolicy(string policy, int multipleSelection, byte[] content)
        {
            string batchDirectory = BatchDirectory();
            string fileName = string.Format("{0:yyyyMMddhhmm}-{1}-{2}.pdf", DateTime.Now, policy, multipleSelection);
            string filePath = Path.Combine(batchDirectory, fileName);
            if(File.Exists(filePath)) { return false; }
            using (Stream outputFile = File.OpenWrite(filePath))
            {
                using (BinaryWriter writer = new BinaryWriter(outputFile))
                {
                    writer.Write(content);
                    writer.Flush();
                    Debug.WriteLine("Policy Saved");
                }
            }
            return true;
        }

        static bool FindBatchFile(string policy)
        {
            string batchDirectory = BatchDirectory();
            Regex policyMatch = new Regex(Regex.Escape(policy), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return Directory.GetFiles(batchDirectory).Where(x=> policyMatch.IsMatch(x)).Any();
        }

        static bool FindBatchFile(string policy, int documentLevel)
        {
            string batchDirectory = BatchDirectory();
            Regex policyMatch = new Regex(Regex.Escape(string.Format("{0}-{1}", policy, documentLevel)), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return Directory.GetFiles(batchDirectory).Where(x => policyMatch.IsMatch(x)).Any();
        }

        static string BatchDirectory()
        {
            string current = Directory.GetCurrentDirectory();
            string path = Path.Combine(current, batchName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }

    public class LossRunCrawlerResult
    {
        public LossRunResult Result {get;set;}
        public string Message {get;set;}
    }

    public enum LossRunResult
    {
        Success,
        Failure,
        Duplicate
    }
}
