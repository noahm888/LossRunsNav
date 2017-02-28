using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LossRunsNavClient.Models
{
    class GlobalStats
    {
        public int BatchesTotal { get; set; }
        public int BatchesRemaining { get; set; }
        public float BatchesCompletePercent { get; set; }
    }
}
