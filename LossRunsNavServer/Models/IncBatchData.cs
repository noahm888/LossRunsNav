using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LossRunsNavServer.Models
{
    public class IncBatchData : BatchData
    {
       public BatchData ConvertToParent()
        {
            return (BatchData)this;
        }

        public IncBatchData Convert(BatchData b)
        {
            this.Id = b.Id;
            this.Key = b.Key;
            this.Data = b.Data;
            this.LastComplete = b.LastComplete;
            this.isAvail = b.isAvail;
            this.isListed = b.isListed;

            return this;
        }
    }
}