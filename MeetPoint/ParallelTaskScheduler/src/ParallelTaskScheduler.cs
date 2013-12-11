using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MeetPoint.Src;
using System.Threading;
using System.ComponentModel;
using log4net;
using System.Reflection;
using Distributor.Service.Src.Util;
using ParallelTaskScheduler.Util;
using Distributor.Service.Src.Contract;

namespace ParallelTaskScheduler.Src
{
    public static class ParallelTaskScheduler
    {
        private static Dictionary<string, ITaskItem> DISTRIBUTED_TASK_MAP = new Dictionary<string, ITaskItem>();

        public static string LocalHostName
        {
            get;
            set;
        }

        public static void Schedule(TaskContainer container)
        {
            Guard.ArgumentNotNull(container, "container");

            ParallelTaskNode currTaskNode = null;

            currTaskNode = container.DeQueueTaskNode();
            do
            {
                // assign meet point
                IMeetPoint meetPoint = AssignMeetPoint(currTaskNode);

                // lanuch tasks parallelly
                LanuchTasks(currTaskNode);

                // wait for all tasks completed
                meetPoint.PostCondArrive(null);
                Log.DebugFormat("taskNode with [{0}] is completed.",
                    currTaskNode.Tasks.Length > 0 ? currTaskNode.Tasks[0].Name : string.Empty);

                // move to next task node
                currTaskNode = container.DeQueueTaskNode();
            } while (currTaskNode != null);
        }

        public static ITaskItem GetDistributedTask(string taskID)
        {
            Guard.ArgumentNotNullOrEmpty(taskID, "taskID");

            lock (DISTRIBUTED_TASK_MAP)
            {
                if (DISTRIBUTED_TASK_MAP.ContainsKey(taskID))
                {
                    return DISTRIBUTED_TASK_MAP[taskID];
                }

                return null;
            }
        }

        private static IMeetPoint AssignMeetPoint(ParallelTaskNode taskNode)
        {
            if (taskNode == null)
            {
                return null;
            }

            string pointID = Guid.NewGuid().ToString();
            bool createdNew = false;

            // the only post condition is created for task scheduller
            IMeetPoint meetPoint = MeetPointFactory.Create(pointID, taskNode.TaskCount, 1,
                Timeout.Infinite, out createdNew);

            foreach (ITaskItem nextTask in taskNode.Tasks)
            {
                nextTask.TaskCompleted += (sender, e) =>
                    {
                        meetPoint.PreCondArrive(null);
                    };
            }

            return meetPoint;
        }

        private static void LanuchTasks(ParallelTaskNode taskNode)
        {
            foreach (ITaskItem nextTask in taskNode.Tasks)
            {
                if (CanBeDistributed(nextTask))
                {
                    LanuchRemoteTask(nextTask);
                }
                else
                {
                    LanuchLocalTask(nextTask);
                }
            }
        }

        private static void LanuchLocalTask(ITaskItem taskItem)
        {
            BackgroundWorker bgWorder = new BackgroundWorker();

            //// use local variable in the anonynouse method or lambda expression
            //// http://www.codeproject.com/Articles/15624/Inside-C-2-0-Anonymous-Methods
            //ITaskItem localNextTask = taskItem;

            bgWorder.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                // set execute type
                taskItem.ExecuteType = TaskExecuteType.Locally;
                taskItem.Status = TaskStatus.Running;
                taskItem.Execute();
            };

            bgWorder.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            {
                if (e.Error != null)
                {
                    Log.ErrorFormat("local task execution error. Message[{0}]\r\nStackTrace[{1}]",
                        e.Error.Message, e.Error.StackTrace);
                    taskItem.Status = TaskStatus.Aborted;
                }
                else
                {
                    taskItem.Status = TaskStatus.Completed;
                }
                
                taskItem.Complete();
            };

            bgWorder.RunWorkerAsync();

            Log.DebugFormat("task [{0}] scheduled (locally).", taskItem.Name);
        }

        public static void LanuchRemoteTask(ITaskItem taskItem)
        {
            BackgroundWorker bgWorder = new BackgroundWorker();

            bgWorder.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                // set execute type
                taskItem.ExecuteType = TaskExecuteType.Remotely;

                // get target node
                string destHostName = ServiceFactory.GetTaskService().GetRelaxedNode();

                // add to pending map
                lock (DISTRIBUTED_TASK_MAP)
                {
                    DISTRIBUTED_TASK_MAP.Add(taskItem.ID, taskItem);
                }

                // box the task
                TransferTaskItem sendTask = TransferHelper.BoxTask(taskItem);

                // set host name
                sendTask.DestNode = destHostName;
                sendTask.SrcNode = LocalHostName;

                // set task type to request
                sendTask.TaskTransferType = TransferType.Request;

                // transfer to master node
                RemoteTaskServer.PendSendTask(sendTask);

                // create meet point for sync - wait for remote response
                bool createdNew;
                IMeetPoint meetPoint = MeetPointFactory.Create(taskItem.ID, out createdNew);
                meetPoint.PostCondArrive(null);
            };

            bgWorder.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            {
                if (e.Error != null)
                {
                    Log.ErrorFormat("remote task execution error. Message[{0}]\r\nStackTrace[{1}]",
                        e.Error.Message, e.Error.StackTrace);
                }

                // complete task
                taskItem.Complete();
                
                // remove from pending map
                lock (DISTRIBUTED_TASK_MAP)
                {
                    DISTRIBUTED_TASK_MAP.Remove(taskItem.ID);
                }
            };

            bgWorder.RunWorkerAsync();

            Log.DebugFormat("task [{0}] scheduled (remotely).", taskItem.Name);
        }

        private static bool CanBeDistributed(ITaskItem taskItem)
        {
            // local node is idle?

            // ask for most powerful node

            // if it is not myself?

            return taskItem.IsDistributable;
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
