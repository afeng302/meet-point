using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using ParallelTaskScheduler.Src;
using System.Reflection;
using log4net;

namespace Distributor.Host
{
    public class CallbackPushTaskService : ICallbackPushTask
    {
        public void Display(string result)
        {
            Console.WriteLine("Reply from server: " + result);
        }


        public void PushTask(string taskID)
        {
            RemoteTaskServer.PendRecvTask(taskID);
        }


        public void PushTaskFailed(string taskID)
        {
            // re-schedule the task
            ITaskItem taskItem = ParallelTaskScheduler.Src.ParallelTaskScheduler.GetDistributedTask(taskID);
            if (taskItem == null)
            {
                Log.ErrorFormat("task [{0}] cannot be found in ParallelTaskScheduler", taskID);
            }
            ParallelTaskScheduler.Src.ParallelTaskScheduler.LanuchRemoteTask(taskItem);
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
