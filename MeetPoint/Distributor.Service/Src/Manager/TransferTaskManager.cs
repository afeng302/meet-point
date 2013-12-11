using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using System.Reflection;
using log4net;
using Distributor.Service.Src.Util;

namespace Distributor.Service.Src.Manager
{
    static class TransferTaskManager
    {
        static Dictionary<string, TransferTaskItem> TRANSFER_TASK_MAP = new Dictionary<string, TransferTaskItem>();

        public static void AddTask(TransferTaskItem taskItem)
        {
            Guard.ArgumentNotNull(taskItem, "taskItem");

            lock (TRANSFER_TASK_MAP)
            {
                if (TRANSFER_TASK_MAP.ContainsKey(taskItem.ID))
                {
                    Log.ErrorFormat("duplicate task [{0}]", taskItem.ID);
                    return;
                }

                TRANSFER_TASK_MAP[taskItem.ID] = taskItem;
            }
        }

        public static void RemoveTask(string taskID)
        {
            Guard.ArgumentNotNullOrEmpty(taskID, "taskID");

            lock (TRANSFER_TASK_MAP)
            {
                if (!TRANSFER_TASK_MAP.ContainsKey(taskID))
                {
                    Log.ErrorFormat("task not found [{0}]", taskID);
                    return;
                }

                TRANSFER_TASK_MAP.Remove(taskID);
            }
        }

        public static TransferTaskItem GetTask(string taskID)
        {
            Guard.ArgumentNotNullOrEmpty(taskID, "taskID");

            lock (TRANSFER_TASK_MAP)
            {
                if (!TRANSFER_TASK_MAP.ContainsKey(taskID))
                {
                    Log.ErrorFormat("task not found [{0}]", taskID);
                    return null;
                }

                return TRANSFER_TASK_MAP[taskID];
            }
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
