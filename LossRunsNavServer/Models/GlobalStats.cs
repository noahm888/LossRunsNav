using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LossRunsNavServer.Models
{
    public class GlobalStats
    {
        public int BatchesTotal { get; set; }
        public int BatchesRemaining { get; set; }
        public float BatchesCompletePercent { get; set; }
    }
}