using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Distributor.Service.Src.Contract;
using log4net;
using System.Reflection;
using ParallelTaskScheduler.Util;
using Distributor.Service.Src.Util;
using MeetPoint.Src;

namespace ParallelTaskScheduler.Src
{
    class RemoteTaskExecutor
    {
        private TransferTaskItem transferTask = null;
        BackgroundWorker worker = new BackgroundWorker();

        public RemoteTaskExecutor(TransferTaskItem transferTaskItem)
        {
            this.transferTask = transferTaskItem;

            this.worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // check task transfer type
            if (this.transferTask.TaskTransferType == TransferType.Request)
            {
                this.ExecuteRequestTask(this.transferTask);
            }
            else if (transferTask.TaskTransferType == TransferType.Response)
            {
                this.ExecuteResponseTask(this.transferTask);
            }
            else
            {
                Log.ErrorFormat("unkonw task transfer type [{0}]", this.transferTask.TaskTransferType);
            }
        }

        public void Run()
        {
            this.worker.RunWorkerAsync();
        }

        /// <summary>
        /// Execute the remote task
        /// </summary>
        /// <param name="requTask"></param>
        private void ExecuteRequestTask(TransferTaskItem requTask)
        {
            Guard.ArgumentNotNull(requTask, "requTask");

            // unbox task
            ITaskItem requTaskItem = TransferHelper.UnboxTask(requTask);

            // increase payload
            ServiceFactory.GetTaskService().IncreasePayload(requTask.DestNode, requTaskItem.PayloadFactor);

            // execute task
            requTaskItem.ExecuteType = TaskExecuteType.Remotely;
            requTaskItem.Status = TaskStatus.Running;
            try
            {
                requTaskItem.Execute();
            }
            catch (Exception e)
            {
                Log.ErrorFormat("requTaskItem.Execute() error. [{0}]", e.Message);
                requTask.Status = TaskStatus.Aborted;
            }

            // set the task status
            requTaskItem.Status = TaskStatus.Completed;

            // decrease payload

            ServiceFactory.GetTaskService().DecreasePayload(requTask.DestNode, requTaskItem.PayloadFactor);
            // box the task
            TransferTaskItem respTask = TransferHelper.BoxTask(requTaskItem);
            
            // prepare task info
            respTask.Status = requTaskItem.Status;
            respTask.DestNode = requTask.SrcNode;
            respTask.SrcNode = requTask.DestNode;
            respTask.TaskTransferType = TransferType.Response;

            // send out result
            RemoteTaskServer.PendSendTask(respTask);
        }

        /// <summary>
        /// Receive the task response (execute result)
        /// </summary>
        /// <param name="respTask"></param>
        private void ExecuteResponseTask(TransferTaskItem respTask)
        {
            Guard.ArgumentNotNull(respTask, "respTask");

            // get the local pending task
            ITaskItem taskItem = ParallelTaskScheduler.GetDistributedTask(respTask.ID);
            if (taskItem == null)
            {
                Log.ErrorFormat("Cannot find original task for the response. [{0}]", respTask.ID);
                return;
            }

            // unbox task
            TransferHelper.UnboxTask(respTask, taskItem);

            // sync task
            bool createdNew;
            IMeetPoint meetPoint = MeetPointFactory.Create(taskItem.ID, out createdNew);
            meetPoint.PreCondArrive(null);
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
