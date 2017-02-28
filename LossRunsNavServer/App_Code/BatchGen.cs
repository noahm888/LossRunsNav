using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Text;
using System.Net.Http;

namespace LossRunsNavServer.App_Code
{
    public class BatchGen
    {
        Logging logger = new Logging(Logging.LogType.Exceptions);

        public List<string> splitDocument(string input, int size, bool isOutputAsCsl)
        {
            List<string> result = new List<string>();
            try
            {
                var delimiters = new char[] { ',', '\n' };
                var stringArray = input.Split(delimiters);

                var listSize = size;
                var totalLines = stringArray.Length;
                int totalLists = (int)totalLines / listSize;
                int remainder = totalLines % listSize;
                int fudgeNumber = (remainder == 0) ? 0 : 1;


                int lineNum = 0;
                for (int i = 0; i < totalLists + fudgeNumber; i++)
                {
                    if (i == totalLists)
                    {
                        var builder = new StringBuilder();
                        for (int j = 0; j < remainder; j++)
                        {
                            if (isOutputAsCsl)
                                builder.Append(stringArray[lineNum] + ",");
                            else
                                builder.AppendLine(stringArray[lineNum]);
                            lineNum++;
                        }
                        result.Add(builder.ToString());
                        builder.Clear();
                    }
                    else
                    {
                        var builder = new StringBuilder();
                        for (int j = 0; j < listSize; j++)
                        {
                            if (isOutputAsCsl)
                                builder.Append(stringArray[lineNum] + ",");
                            else
                                builder.AppendLine(stringArray[lineNum]);
                            lineNum++;
                        }
                        result.Add(builder.ToString());
                        builder.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(typeof(BatchGen), ex, false, true);
            }

            return result;
        }


        public bool saveDocument(string document, string serverPath, string dirName, string name)
        {
            try
            {
                var path = serverPath + @"\Generated_Lists" + @"\" + dirName;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path = path + @"\" + name + @".txt";
                if (File.Exists(path))
                    return false;

                var writer = new StreamWriter(path, false);
                writer.Write(document);
                writer.Close();

                return true;
            }
            catch (Exception ex)
            {
                logger.Log(typeof(BatchGen), ex, false, true);
                throw ex;
            }
        }
    }
}