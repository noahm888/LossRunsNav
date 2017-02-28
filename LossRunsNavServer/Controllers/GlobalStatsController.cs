using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LossRunsNavServer.Models;
using LossRunsNavServer.App_Code;
using System.Threading.Tasks;

namespace LossRunsNavServer.Controllers
{
    public class GlobalStatsController : ApiController
    {

        [HttpGet]
        public async Task<GlobalStats> GetGlobalStats()
        {
            GlobalStats stats = new GlobalStats();
            int? tmp = await BatchManager.Instance.GetTotal();
            if (tmp != null)
                stats.BatchesTotal = (int)tmp;
            else
                stats.BatchesTotal = 0;
            stats.BatchesRemaining = await BatchManager.Instance.GetCount();
            stats.BatchesCompletePercent = (stats.BatchesTotal == 0) ? 0.0F : (stats.BatchesTotal - stats.BatchesRemaining) / stats.BatchesTotal;

            return stats;
        }
    }
}
