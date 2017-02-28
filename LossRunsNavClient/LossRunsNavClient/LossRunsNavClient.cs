using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LossRunsNavClient.Request;
using LossRunsNavClient.Models;
using LossRunsNavClient.Monitors;
using System.Diagnostics;
using Newtonsoft.Json;

using DebugUtils;

namespace LossRunsNavClient
{
    public partial class LossRunsNavClient : Form
    {
        bool isRunning = false;
        bool isFirst = true;
        bool isCustomConfig;
        bool isAdvConfig;
        bool isBatchReady;
        static string configDirPath = Directory.GetCurrentDirectory() + @"\config\";
        static string configPath = configDirPath + "UserConfig.json";
        bool paused;

        // Blocking
        BlockingCollection<WebBrowser> BrowserCollection;
        BlockingCollection<WebBrowser> PendingBrowsers;
        BlockingCollection<BatchProcessing> BatchCollection;

        // Config Options
        UserConfig config;

        // Logging
        ErrorLogger frontLog;
        ErrorLogger backLog;
        bool logWriteFinished;
        bool backLogWriteFinished;

        // Constructor
        public LossRunsNavClient()
        {
            InitializeComponent();
            BatchCollection = new BlockingCollection<BatchProcessing>();
            BrowserCollection = new BlockingCollection<WebBrowser>();
            isCustomConfig = false;
            isAdvConfig = false;
            isBatchReady = false;
            paused = false;
            frontLog = new ErrorLogger("FrontLog.txt", true, true);
            backLog = new ErrorLogger("BackLog.txt", true, true);

            config = (File.Exists(configPath)) ? UserConfig.Load(configPath) : new UserConfig(true, configDirPath, configPath);

            PendingBrowsers = new BlockingCollection<WebBrowser>()
            {
                Output1,
                Output2,
                Output3,
                Output4,
                Output5,
                Output6,
                Output7,
                Output8,
            };

        }

        // Config Handler
        private void btnConfig_Click(object sender, EventArgs e)
        {
            if (!isCustomConfig)
                enableBasicConfig();

            if (!config.configToggle)
            {
                lblHostUrl.Visible = true;
                lblProcessingLevel.Visible = true;
                tbHostUrl.Visible = true;
                gbProcessingLevel.Visible = true;
                rbLow.Visible = true;
                rbHigh.Visible = true;
                btnAdvConfig.Visible = true;
                
            }
            else
            {
                tbGlobalTimeout.Visible = false;
                tbHostUrl.Visible = false;
                tbMaxCpu.Visible = false;
                tbMaxMem.Visible = false;
                tbMaxWindows.Visible = false;
                lblGlobalTimeout.Visible = false;
                lblHostUrl.Visible = false;
                lblMaxCpu.Visible = false;
                lblMaxMem.Visible = false;
                lblMaxWindows.Visible = false;
                lblProcessingLevel.Visible = false;
                btnReset.Visible = false;
                btnAdvConfig.Visible = false;
                gbProcessingLevel.Visible = false;
                rbLow.Visible = false;
                rbHigh.Visible = false;
            }

            config.configToggle = !config.configToggle;
        }

        private void btnAdvConfig_Click(object sender, EventArgs e)
        {
            tbGlobalTimeout.Text = config.globalTimeout.ToString();
            tbMaxCpu.Text = config.maxCpu.ToString();
            tbMaxMem.Text = config.maxMem.ToString();
            tbMaxWindows.Text = config.maxWindows.ToString();

            tbGlobalTimeout.Visible = true;
            tbMaxCpu.Visible = true;
            tbMaxMem.Visible = true;
            tbMaxWindows.Visible = true;
            lblGlobalTimeout.Visible = true;
            lblMaxCpu.Visible = true;
            lblMaxMem.Visible = true;
            lblMaxWindows.Visible = true;
            btnReset.Visible = true;
            isAdvConfig = true;
        }


        // Run
        private async void btnStart_Click(object sender, EventArgs e)
        {
            //btnCancel.Visible = true;
            int batchIndex = 0;
            bool finishBatch = false;
            isRunning = true;
            bool isNewBatch = true;
            BatchProcessing batch = null;
            disableConfig();
            await GetBatchCollectionAsync();
            while (isRunning)
            {
                //while(paused)
                //{
                //    int pau = 0;
                //    if (pau == 0)
                //        tbLog.Text += "Paused";
                //    Task.Delay(100);
                //    pau = 1;
                //    btnStart.Text = "Start";
                //}
                if (isNewBatch)
                {
                    if (BatchCollection.Count > 0)
                    {
                        batch = BatchCollection.Take();
                        isNewBatch = false;
                    }
                }

                if (batch.LastComplete > batchIndex)
                {
                    finishBatch = true;
                    while (finishBatch)
                    {
                        var dispose = batch.DataArray.Take();
                        batchIndex++;
                        if (batchIndex == batch.LastComplete)
                            finishBatch = false;
                        await Task.Delay(5);
                        dispose = null;
                    }
                }


                WebBrowser browser = null;
                if (BrowserCollection.Count < 1)
                    browser = await GetBrowser();
                string policy = null;
                if (batch.DataArray.Count > 0)
                    policy = batch.DataArray.Take();

                if (browser != null && policy != null)
                {
                    Crawler.LossRunCrawlerResult result;
                    int i = 0;
                    while (i < 10)
                    {
                        try
                        {
                            tbLog.Text += "Starting WebCrawler\n";
                            result = await StartCrawler(browser, policy, batch.Key);

                            if (result.Result == Crawler.LossRunResult.Success)
                            {
                                tbLog.Text += string.Format("Policy {0} Result: Success", policy);
                                tbLog.Text += "\n";
                                lblBatchRemain.Text = string.Format("Batch Remaining: {0}", batch.DataArray.Count.ToString());
                                PendingBrowsers.Add(browser);
                            }
                            if (result.Result == Crawler.LossRunResult.Duplicate)
                            {
                                tbLog.Text += string.Format("Policy {0} Result: Duplicate", policy);
                                tbLog.Text += "\n";
                                lblBatchRemain.Text = string.Format("Batch Remaining: {0}", batch.DataArray.Count.ToString());
                                PendingBrowsers.Add(browser);
                            }
                            if (result.Result == Crawler.LossRunResult.Failure)
                            {
                                tbLog.Text += string.Format("Policy {0} Result: Failure", policy);
                                tbLog.Text += "\n";
                                lblBatchRemain.Text = string.Format("Batch Remaining: {0}", batch.DataArray.Count.ToString());
                                PendingBrowsers.Add(browser);
                            }
                            i = 15;
                            batchIndex++;
                        }
                        catch
                        {
                            batch.LastComplete = batchIndex;
                            if (i < 3)
                                await Task.Delay(config.globalTimeout);
                            if (i >= 3 && i < 7)
                                await Task.Delay(config.globalTimeout * 2);
                            if (i >= 7 && i < 10)
                                await Task.Delay(config.globalTimeout * 60 * 60);
                        }
                        finally
                        {
                            i++;
                        }
                    }

                    var upReq = new BatchRequest(tbHostUrl.Text);
                    var incBatch = new BatchData();
                    incBatch.Id = batch.Id;
                    incBatch.Key = batch.Key;
                    incBatch.Data = batch.Data;
                    incBatch.LastComplete = batch.LastComplete;
                    incBatch.isListed = batch.isListed;

                    if (i >= 10 && i < 15)
                    {
                        incBatch.isAvail = true;
                        await upReq.UpdateBatch(incBatch);
                    }
                    else
                    {
                        incBatch.isAvail = false;
                        await upReq.UpdateBatch(incBatch);
                    }
                }
                else
                    isRunning = false;

                var req = new BatchRequest(tbHostUrl.Text);
                var stats = await req.GetStats();
                lblGlobalBatRem.Text = "Batches Remaining: " + stats.BatchesRemaining.ToString();
                lblGlobalPer.Text = "Percent Complete: " + (stats.BatchesCompletePercent * 100).ToString();

                if (batch.DataArray.Count == 0)
                {
                    // Post results to server
                    // AddCode

                    isNewBatch = true;
                }

                if (batch.DataArray.Count < 10)
                {
                    isBatchReady = true;
                }

                if (isBatchReady && !isNewBatch)
                {
                    isBatchReady = false;
                    int j = 0;
                    while (j < 3)
                    {
                        try
                        {
                            if (batch.DataArray.Count < 10)
                            {
                                await GetBatchCollectionAsync();
                                j = 4;
                            }
                        }
                        catch
                        {
                            await Task.Delay(config.globalTimeout);
                            Debug.WriteLine("Failed - Trying Again");
                            tbLog.Text += "Failed - Trying Again\n";
                            j++;
                        }
                    }
                    if (j == 3)
                        isRunning = false;
                }
                
            }
            tbLog.Text += "All Finished!";
        }

        private async Task<Crawler.LossRunCrawlerResult> StartCrawler(WebBrowser browser, string policy, string name)
        {
            return Crawler.LossRunCrawler.Process(browser, policy, name);
        }

        // Policy List Control
        private async Task GetBatchCollectionAsync()
        {
            var batchReq = new BatchRequest(config.hostUrl);
            var batch = await batchReq.GetNextBatchAsync();

            Debug.WriteLine("Requested New Batch");

            // add something to keep track of these

            string[] batchList = batch.Data.Replace("\r", "").Split(',');

            var batchProc = new BatchProcessing();
            batchProc.Id = batch.Id;
            batchProc.Key = batch.Key;
            batchProc.Data = batch.Data;
            batchProc.LastComplete = batch.LastComplete;
            batchProc.isListed = batch.isListed;
            BlockingCollection<string> tmpBlock = new BlockingCollection<string>();

            foreach (var b in batchList)
            {
                tmpBlock.Add(b);
            }
            batchProc.DataArray = tmpBlock;
            BatchCollection.Add(batchProc);
            tbLog.Text += string.Format("Policy Collection Added {0} items", batchList.Length.ToString());
            tbLog.Text += "\n";
        }

        // Browser Control
        private async Task<WebBrowser> GetBrowser()
        {
            if (!isFirst)
                await LoadBrowser();

            if (BrowserCollection.Count > 0)
            {
                tbLog.Text += string.Format("Brower Loaded: {0} remain", (BrowserCollection.Count - 1).ToString());
                tbLog.Text += "\n";
                return BrowserCollection.Take();
            }
            else
            {
                if (isFirst)
                {
                    isFirst = false;
                    BrowserCollection.Add(PendingBrowsers.Take());
                    return BrowserCollection.Take();
                }
                return null;
            }

        }

        private async Task LoadBrowser()
        {
            if (config.maxCpu < 80)
            {
                var monitor = new SystemMonitor(20, config.maxCpu, config.maxCpu, config.maxMem, config.maxMem, config.globalTimeout);
                if (await monitor.CheckCpuUsage())
                {
                    if (BrowserCollection.Count < config.maxWindows)
                        activateBrowser(PendingBrowsers.Take());
                }
                else
                {
                    if (BrowserCollection.Count > 1)
                    {
                        var browser = BrowserCollection.Take();
                        await deactivateBrowser(browser);
                    }
                }
            }
            else
            {
                if (BrowserCollection.Count < config.maxWindows)
                    await activateBrowser(PendingBrowsers.Take());
            }
        }

        // TextBox handlers
        private void tbHostUrl_TextChanged(object sender, EventArgs e)
        {
            config.hostUrl = tbHostUrl.Text;
        }

        private void tbMaxWindows_TextChanged(object sender, EventArgs e)
        {
            int maxWindows;
            if (int.TryParse(tbMaxWindows.Text, out maxWindows))
            {
                if (maxWindows < 1 || maxWindows > 8)
                    maxWindows = 8;
                config.maxWindows = maxWindows;
            }

            if (isCustomConfig)
            {
                if (tbMaxWindows.Text == string.Empty)
                {
                    enableBasicConfig();
                    isCustomConfig = false;
                }
                else
                {
                    disableBasicConfig();
                    isCustomConfig = true;
                }
            }
        }

        private void tbGlobalTimeout_TextChanged(object sender, EventArgs e)
        {
            int globalTimeout;
            if (int.TryParse(tbGlobalTimeout.Text, out globalTimeout))
            {
                if (globalTimeout < 1000 || globalTimeout > 10000)
                    globalTimeout = 5000;
                config.globalTimeout = globalTimeout;
            }
        }

        private void tbMaxCpu_TextChanged(object sender, EventArgs e)
        {
            float maxCpu;
            if (float.TryParse(tbMaxCpu.Text, out maxCpu))
            {
                if (maxCpu < 30.0)
                    maxCpu = 30.0F;
                config.maxCpu = maxCpu;
            }

            if (isCustomConfig)
            {
                if (tbMaxCpu.Text == string.Empty)
                {
                    enableBasicConfig();
                    isCustomConfig = false;
                }
                else
                {
                    disableBasicConfig();
                    isCustomConfig = true;
                }
            }
        }

        private void tbMaxMem_TextChanged(object sender, EventArgs e)
        {
            float maxMem;
            if (float.TryParse(tbMaxMem.Text, out maxMem))
            {
                if (maxMem < 25.0)
                    maxMem = 25.0F;
                config.maxMem = maxMem;
            }

            if (isCustomConfig)
            {
                if (tbMaxMem.Text == string.Empty)
                {
                    enableBasicConfig();
                    isCustomConfig = false;
                }
                else
                {
                    disableBasicConfig();
                    isCustomConfig = true;
                }
            }
        }

        // Helper methods
        private void disableConfig()
        {
            btnConfig.Enabled = false;
            tbGlobalTimeout.Enabled = false;
            tbHostUrl.Enabled = false;
            tbMaxCpu.Enabled = false;
            tbMaxMem.Enabled = false;
            tbMaxWindows.Enabled = false;

            tbGlobalTimeout.BackColor = Color.Gray;
            tbHostUrl.BackColor = Color.Gray;
            tbMaxCpu.BackColor = Color.Gray;
            tbMaxMem.BackColor = Color.Gray;
            tbMaxWindows.BackColor = Color.Gray;
        }

        private void enableConfig()
        {
            btnConfig.Enabled = true;
            tbGlobalTimeout.Enabled = true;
            tbHostUrl.Enabled = true;
            tbMaxCpu.Enabled = true;
            tbMaxMem.Enabled = true;
            tbMaxWindows.Enabled = true;

            tbGlobalTimeout.BackColor = Color.White;
            tbHostUrl.BackColor = Color.White;
            tbMaxCpu.BackColor = Color.White;
            tbMaxMem.BackColor = Color.White;
            tbMaxWindows.BackColor = Color.White;
        }

        private void disableBasicConfig()
        {
            gbProcessingLevel.Enabled = false;
            rbLow.Enabled = false;
            rbHigh.Enabled = false;

            rbLow.BackColor = Color.Gray;
            rbHigh.BackColor = Color.Gray;
        }

        private void enableBasicConfig()
        {
            gbProcessingLevel.Enabled = true;
            rbLow.Enabled = true;
            rbHigh.Enabled = true;

            rbLow.BackColor = Color.White;
            rbHigh.BackColor = Color.White;
        }

        private async Task activateBrowser(WebBrowser browser)
        {
            browser.Visible = true;
            BrowserCollection.Add(browser);
        }

        private async Task deactivateBrowser(WebBrowser browser)
        {
            browser.Visible = false;
            var rmvBrowser = BrowserCollection.Take();
            PendingBrowsers.Add(rmvBrowser);
        }

        // Logging
        private async Task LoadLog()
        {
            logWriteFinished = false;
            await SaveLog();

            using (var reader = new StreamReader(frontLog.GetPath(true)))
            {

                while (!logWriteFinished)
                {
                    await Task.Delay(100);
                }


            }
        }

        private async Task SaveLog()
        {
            frontLog.Write();
            logWriteFinished = true;
        }

        private async Task LoadBackLog()
        {
            backLogWriteFinished = false;
            await SaveBackLog();

            using (var reader = new StreamReader(backLog.GetPath(true)))
            {

                while (!backLogWriteFinished)
                {
                    await Task.Delay(100);
                }


            }
        }

        private async Task SaveBackLog()
        {
            backLog.Write();
            backLogWriteFinished = true;
        }

        // Basic Config Controls
        private void rbLow_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLow.Checked)
            {
                if (!isAdvConfig)
                    isCustomConfig = false;
                config.maxWindows = 2;
                config.maxCpu = 30.0F;
                config.maxMem = 20.0F;
                tbMaxWindows.Text = config.maxWindows.ToString();
                tbMaxCpu.Text = config.maxCpu.ToString();
                tbMaxMem.Text = config.maxMem.ToString();
            }
        }

        private void rbHigh_CheckedChanged(object sender, EventArgs e)
        {
            if (rbHigh.Checked)
            {
                if (!isAdvConfig)
                    isCustomConfig = false;
                config.maxWindows = 8;
                config.maxCpu = 100.0F;
                config.maxMem = 100.0F;
                tbMaxWindows.Text = config.maxWindows.ToString();
                tbMaxCpu.Text = config.maxCpu.ToString();
                tbMaxMem.Text = config.maxMem.ToString();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            config.Defaults();
            tbMaxWindows.Text = config.maxWindows.ToString();
            tbMaxCpu.Text = config.maxCpu.ToString();
            tbMaxMem.Text = config.maxMem.ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (!paused)
            {
                btnStart.Text = "Paused";
                tbLog.Text += "Waiting for task to Complete";
                paused = true;
            }
            else
            {
                btnStart.Text = "Start";
                tbLog.Text += "Resuming";
                paused = false;
            }
        }


    }

    class UserConfig
    {
        private string dirPath;
        private string jsonPath;

        private bool _configToggle;
        private string _hostUrl;
        private int _maxWindows;
        private int _globalTimeout;
        private float _maxCpu;
        private float _maxMem;

        // Config Options
        public bool configToggle
        {
            get { return _configToggle; }
            set
            {
                _configToggle = value;
                this.Save();
            }
        }
        public string hostUrl
        {
            get { return _hostUrl; }
            set
            {
                _hostUrl = value;
                this.Save();
            }
        }
        public int maxWindows
        {
            get { return _maxWindows; }
            set
            {
                _maxWindows = value;
                this.Save();
            }
        }
        public int globalTimeout
        {
            get { return _globalTimeout; }
            set
            {
                _globalTimeout = value;
                this.Save();
            }
        }
        public float maxCpu
        {
            get { return _maxCpu; }
            set
            {
                _maxCpu = value;
                this.Save();
            }
        }
        public float maxMem
        {
            get { return _maxMem; }
            set
            {
                _maxMem = value;
                this.Save();
            }
        }

        // Constructors
        public UserConfig()
        {
            dirPath = Directory.GetCurrentDirectory() + @"\config\";
            jsonPath = dirPath + "UserConfig.json";
        }
        
        public UserConfig(string path, string jpath)
        {
            dirPath = path;
            jsonPath = jpath;
            this.Reset();
        }

        public UserConfig(bool isDefault, string path, string jpath)
        {
            dirPath = path;
            jsonPath = jpath;
            if (isDefault)
                this.Defaults();
            else
                this.Reset();
        }

        // Async Methods
        public async Task Reset()
        {
            if (File.Exists(jsonPath))
            {
                var savedConfig = Load(jsonPath);
                this._configToggle = savedConfig.configToggle;
                this._hostUrl = savedConfig.hostUrl;
                this._maxWindows = savedConfig.maxWindows;
                this._globalTimeout = savedConfig.globalTimeout;
                this._maxCpu = savedConfig.maxCpu;
                this._maxMem = savedConfig.maxMem;
                this.Save();
            }
            else
            {
                configToggle = false;
                if (hostUrl == null)
                    hostUrl = @"https://dev.btisinc.com/LossRuns/";
                if (maxWindows < 1)
                    maxWindows = 8;
                if (globalTimeout < 1)
                    globalTimeout = 5000;
                if (maxCpu < 1)
                    maxCpu = 30.0F;
                if (maxMem < 1)
                    maxMem = 25.0F;
                this.Save();
            }
        }

        private async Task Save()
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            bool tmp = this._configToggle;
            this._configToggle = false;
            using (var writer = new StreamWriter(jsonPath))
            {
                writer.Write(JsonConvert.SerializeObject(this));
            }
            this._configToggle = tmp;
        }

        // Sync Methods
        public void Defaults()
        {
            configToggle = false;
            hostUrl = @"https://dev.btisinc.com/LossRuns/";
            maxWindows = 8;
            globalTimeout = 5000;
            maxCpu = 30.0F;
            maxMem = 25.0F;
            this.Save();
        }

        // static Methods
        public static UserConfig Load(string path)
        {
            UserConfig tmp;
            using (var reader = new StreamReader(path))
            {
                tmp = JsonConvert.DeserializeObject<UserConfig>(reader.ReadToEnd());
            }

            return tmp;
        }
    }

    class BatchProcessing
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Data { get; set; }
        public int LastComplete { get; set; }
        public BlockingCollection<string> DataArray { get; set; }
        public bool isListed { get; set; }
    }
}
