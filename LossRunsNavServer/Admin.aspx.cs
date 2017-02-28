using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using LossRunsNavServer.App_Code;
using LossRunsNavServer.Models;
using System.Threading.Tasks;

namespace LossRunsNavServer
{
    public partial class Admin : System.Web.UI.Page
    {
        Logging logger;
        string fileName;
        int batchSize;
        string path = HttpContext.Current.Server.MapPath(".") + "\\Data\\";
        BatchGen gen;

        protected void Page_Load(object sender, EventArgs e)
        {
            logger = new Logging(Logging.LogType.Results);
            fileName = tbFileName.Text;
            if (fileName == null)
                fileName = "Batch";

            batchSize = int.Parse(tbBatchSize.Text);
            if (batchSize == 0)
                batchSize = 1000;

            BatchManager.StartManager();
            gen = new BatchGen();
        }

        protected async void btnProcess_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                var reader = new StreamReader(FileUpload1.FileContent);
                var document = reader.ReadToEnd();
                reader.Close();
                string fileName = tbFileName.Text;
                bool success = gen.saveDocument(document, path, fileName, fileName);
                if (success)
                {
                    int batchSize = 1000;
                    var isCustomBatch = int.TryParse(tbBatchSize.Text, out batchSize);
                    if (isCustomBatch)
                        lblBatchSize.Text = string.Format("Batch Size {0}", batchSize);
                    var docList = gen.splitDocument(document, batchSize, true);

                    int docCount = 1;
                    List<BatchManager.BatchResult> results = null;
                    foreach (var doc in docList)
                    {
                        var batch = new BatchData();
                        batch.Id = docCount;
                        var name = fileName + "_" + docCount.ToString();
                        batch.Key = name;
                        batch.Data = doc.Replace("\r", "");
                        batch.LastComplete = 0;
                        batch.isAvail = true;
                        gen.saveDocument(doc, path, fileName, name);
                        docCount++;
                        results = await BatchManager.Instance.Add(batch);
                    }

                    if (!results.Contains(BatchManager.BatchResult.Success))
                    {
                        foreach (var result in results)
                        {
                            logger.Log(typeof(Admin), null, null , result);
                        }
                    }

                    BatchManager.Instance.SetTotal(docCount);

                    lblProcess.ForeColor = System.Drawing.Color.Green;
                    lblProcess.Text = "Done: Sucess!";
                }

                else
                {
                    lblFileUpload.ForeColor = System.Drawing.Color.Red;
                    lblFileUpload.Text = "Upload Failed: File Already Exists";
                }
            }
            else
            {
                lblFileUpload.ForeColor = System.Drawing.Color.Red;
                lblFileUpload.Text = "Upload Failed: File not found";
            }
        }

        protected void tbBatchSize_TextChanged(object sender, EventArgs e)
        {
            batchSize = int.Parse(tbBatchSize.Text);
        }

        protected void tbFileName_TextChanged(object sender, EventArgs e)
        {
            fileName = tbFileName.Text;
        }

        protected async void btnReset_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                lblReset.Text = "Batches Deleted";
            }
            else
            {
                lblReset.Text = "No Batches";
            }
            var fPath = await BatchManager.Instance.GetPath();
            if (File.Exists(fPath))
            {
                File.Delete(fPath);
                lblReset2.Text = "Batch Manager Cleared";
            }
            else
            {
                lblReset.Text = "No Batch Manager";
            }
            
        }
    }
}