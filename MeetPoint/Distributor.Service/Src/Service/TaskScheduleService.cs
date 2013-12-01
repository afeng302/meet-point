using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using Distributor.Service.Src.Util;
using System.Reflection;
using log4net;

namespace Distributor.Service.Src.Service
{
    public class TaskScheduleService : ITaskScheduleService
    {
        private Dictionary<string, TransferTaskItem> transferTaskMap = new Dictionary<string, TransferTaskItem>();

        public void ReportFactor(int capacityFactor)
        {
            throw new NotImplementedException();
        }

        public void PushTask(TransferTaskItem taskItem)
        {
            Guard.ArgumentNotNull(taskItem, "taskItem");

            lock (this.transferTaskMap)
            {
                if (this.transferTaskMap.ContainsKey(taskItem.ID))
                {
                    Log.ErrorFormat("duplicate task [{0}]", taskItem.ID);
                    return;
                }

                this.transferTaskMap[taskItem.ID] = taskItem;
                Log.InfoFormat("task was pushed successfully [{0}]", taskItem.ID);
            }

            // inform the receiver 

        }

        public TransferTaskItem PullTask(string taskID)
        {
            Guard.ArgumentNotNullOrEmpty(taskID, "taskID");

            TransferTaskItem taskItem = null;

            lock (this.transferTaskMap)
            {
                if (!this.transferTaskMap.ContainsKey(taskID))
                {
                    Log.ErrorFormat("task not found [{0}]", taskID);
                    return null;
                }

                taskItem = this.transferTaskMap[taskID];

                this.transferTaskMap.Remove(taskID);
                Log.InfoFormat("task was pulled and deleted from master node [{0}]", taskID);
            }

            return taskItem;
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
