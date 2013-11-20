using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using log4net;
using System.Reflection;
using Distributor.Service.Src.Contract;
using ParallelTaskScheduler.Util;
using Distributor.Service.Src.Util;

namespace ParallelTaskScheduler.Src
{
    class TaskResult
    {
        public string TaskID { get; set; }

        public TaskStatus Status { get; set; }

        public string Message { get; set; }
    }

    class RemoteTaskTransferor
    {
        private ITaskItem taskItem = null;
        private BackgroundWorker bkWorker = new BackgroundWorker();

        public RemoteTaskTransferor(ITaskItem taskItem)
        {
            Guard.ArgumentNotNull(taskItem, "taskItem");

            this.taskItem = taskItem;

            this.bkWorker.DoWork += new DoWorkEventHandler(bkWorker_DoWork);
            this.bkWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bkWorker_RunWorkerCompleted);
        }

        public void StartAsync()
        {
            this.bkWorker.RunWorkerAsync();
        }

        public event RunWorkerCompletedEventHandler TaskCompleted
        {
            add
            {
                this.bkWorker.RunWorkerCompleted += value;
            }
            remove
            {
                this.bkWorker.RunWorkerCompleted -= value;
            }
        }

        void bkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Log.ErrorFormat("task execution error. Message[{0}]\r\nStackTrace[{1}]",
                        e.Error.Message, e.Error.StackTrace);
            }
        }

        void bkWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // box task
            TransferTaskItem transferTask = TransferHelper.BoxTask(this.taskItem);

            // transfer task to master node
            

            // wait for result
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
