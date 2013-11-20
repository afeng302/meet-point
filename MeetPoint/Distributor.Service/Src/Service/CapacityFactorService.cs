using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;

namespace Distributor.Service.Src.Service
{
    public class CapacityFactorService : ICapacityFactor
    {
        public void ReportFactor(int capacityFactor)
        {
            throw new NotImplementedException();
        }

        public void PushTask(TransferTaskItem taskItem)
        {
            throw new NotImplementedException();
        }

        public TransferTaskItem PullTask(string taskID)
        {
            throw new NotImplementedException();
        }
    }
}
