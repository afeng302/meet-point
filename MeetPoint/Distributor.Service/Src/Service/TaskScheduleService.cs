using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using Distributor.Service.Src.Util;
using System.Reflection;
using log4net;
using Distributor.Service.Src.Manager;

namespace Distributor.Service.Src.Service
{
    public class TaskScheduleService : ITaskScheduleService
    {
        public void ReportFactor(int capacityFactor)
        {
            throw new NotImplementedException();
        }

        public void PushTask(TransferTaskItem taskItem)
        {
            Guard.ArgumentNotNull(taskItem, "taskItem");

            // add task
            TransferTaskManager.AddTask(taskItem);

            Log.InfoFormat("task was pushed successfully [{0}]", taskItem.ID);

            // inform the receiver 
            ICallbackPushTask callback = CallbackChannelManager.GetCallbackChannel(taskItem.DestNode);
            if (callback == null)
            {
                Log.ErrorFormat("dest node [{0}] channel cannot be found.", taskItem.DestNode);

                // inform error to the source node
                callback = CallbackChannelManager.GetCallbackChannel(taskItem.SrcNode);
                if (callback == null)
                {
                    Log.ErrorFormat("src node [{0}] channel cannot be found.", taskItem.SrcNode);
                    return;
                }
                callback.PushTaskFailed(taskItem.ID);
                return;
            }
            callback.PushTask(taskItem.ID);
        }

        public TransferTaskItem PullTask(string taskID)
        {
            Guard.ArgumentNotNullOrEmpty(taskID, "taskID");

            // get task
            TransferTaskItem taskItem = TransferTaskManager.GetTask(taskID);
            Log.InfoFormat("task was pulled [{0}]", taskID);

            // remove task
            TransferTaskManager.RemoveTask(taskID);
            Log.InfoFormat("task was deleted from master node [{0}]", taskID);

            return taskItem;
        }

        public string GetRelaxedNode()
        {
            return PayloadManager.GetRelaxedNode();
        }

        public void IncreasePayload(string hostName, int payloadFactor)
        {
            Guard.ArgumentNotNullOrEmpty(hostName, "hostName");

            PayloadManager.IncreasePayload(hostName, payloadFactor);
        }

        public void DecreasePayload(string hostName, int payloadFactor)
        {
            Guard.ArgumentNotNullOrEmpty(hostName, "hostName");

            PayloadManager.DecreasePayload(hostName, payloadFactor);
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
