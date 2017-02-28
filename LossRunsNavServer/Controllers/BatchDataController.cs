using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

using LossRunsNavServer.Models;
using LossRunsNavServer.App_Code;
using System.Threading.Tasks;

namespace LossRunsNavServer.Controllers
{
    public class BatchDataController : ApiController
    {
        Logging logger = new Logging(Logging.LogType.Exceptions);
        BatchData[] batches = new BatchData[1];

        public async Task<IEnumerable<BatchData>> GetAllBatches()
        {
            try
            {
                batches[0] = await GetNext(BatchCollectionType.Priority);
                if (batches[0] == null)
                    batches[0] = await GetNext(BatchCollectionType.Main);
                
            }
            catch (Exception ex)
            {
                await logger.Log(typeof(BatchGen), ex, false, true);
            }
            return batches;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetBatchData(int id)
        {
            id = 0;
            BatchData batch = null;
            try
            { 
                if (batches == null)
                    batch = await GetNext(BatchCollectionType.Priority);
                if (batches[0] == null)
                    batch = await GetNext(BatchCollectionType.Priority);
                else
                    batch = batches[0];

                if (batch == null)
                    batch = await GetNext(BatchCollectionType.Main);
            }
            catch (Exception ex)
            {
                await logger.Log(typeof(BatchGen), ex, false, true);
            }
            if (batch == null)
            {
                return NotFound();
            }
            return Ok(batch);
        }

        //[HttpPost]
        //public async Task<BatchManager.BatchResult> PostBatchData([FromBody]BatchData batch)
        //{
        //    //var batch = JsonConvert.DeserializeObject<BatchData>(batchStr);
        //    try
        //    {
        //        return await BatchManager.Instance.Update(batch);
        //    }
        //    catch
        //    {
        //        return BatchManager.BatchResult.CouldNotSave;
        //    }
        //}

        private async Task<BatchData> GetNext(BatchCollectionType type)
        {
            if (type == BatchCollectionType.Main)
                return await BatchManager.Instance.Take();
            else if (type == BatchCollectionType.Priority)
                return await BatchManager.Instance.TakeInc();
            else
                return null;
        }
    }

    public enum BatchCollectionType
    {
        Main,
        Priority
    }
}
