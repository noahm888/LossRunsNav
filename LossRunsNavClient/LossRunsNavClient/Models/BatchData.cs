using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LossRunsNavClient.Models
{
    class BatchData
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Data { get; set; }
        public int LastComplete { get; set; }
        public bool isAvail { get; set; }
        public bool isListed { get; set; }
    }
}
