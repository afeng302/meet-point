using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Distributor.Service.Src.Contract
{
    [ServiceContract]
    public interface ITaskScheduleService
    {
        [OperationContract]
        void ReportFactor(int capacityFactor);

        [OperationContract]
        void PushTask(TransferTaskItem taskItem);

        [OperationContract]
        TransferTaskItem PullTask(string taskID);
    }
}
