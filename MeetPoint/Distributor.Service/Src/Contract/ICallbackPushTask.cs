using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Distributor.Service.Src.Contract
{
    public interface ICallbackPushTask
    {
        [OperationContract(IsOneWay=true)]
        void Display(string result);

        /// <summary>
        /// push task to destination node
        /// </summary>
        /// <param name="taskID"></param>
        [OperationContract(IsOneWay = true)]
        void PushTask(string taskID);

        /// <summary>
        /// report error to source node
        /// </summary>
        /// <param name="taskID"></param>
        [OperationContract(IsOneWay = true)]
        void PushTaskFailed(string taskID);
    }
}
