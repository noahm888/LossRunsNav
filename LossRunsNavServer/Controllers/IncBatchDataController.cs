using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using LossRunsNavServer.Models;
using LossRunsNavServer.App_Code;
using System.Threading.Tasks;
using LossRunsNavServer.App_Code;

namespace LossRunsNavServer.Controllers
{
    public class IncBatchDataController : ApiController
    {
        Logging logger = new Logging(Logging.LogType.Update);
        Logging resultsLogger = new Logging(Logging.LogType.Results);
        // GET: api/IncBatchData
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/IncBatchData/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/IncBatchData
        public async Task Post([FromBody]BatchData value)
        {
            List<BatchManager.BatchResult> results = null;
            if (value.isAvail)
            {
                results = await BatchManager.Instance.UpdateInc(value.Id, value.isAvail, value.LastComplete);
                if (!results.Contains(BatchManager.BatchResult.Updated))
                {
                    var inc = new IncBatchData();
                    bool success = await BatchManager.Instance.AddToQueue(inc.Convert(value));

                    if (success)
                    {
                        BatchManager.Instance.LoadQueue();
                        await logger.Log(value, "{0} Was not Updated. Adding to queue.");
                    }
                }
            }
            else
            {
                //Log results
                await logger.Log(value, null);
            }

            if (results != null)
            {
                foreach (var r in results)
                {
                    await resultsLogger.Log(typeof(IncBatchDataController), value, null, r);
                }
            }
            else
                resultsLogger.Log(typeof(IncBatchDataController), value, null, BatchManager.BatchResult.NullResult);
        }

        // PUT: api/IncBatchData/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/IncBatchData/5
        public void Delete(int id)
        {
        }

    }
}
