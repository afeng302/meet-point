using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParallelTaskScheduler.Src;
using System.Threading;
using log4net;
using System.Reflection;

namespace ParallelTaskScheduler.Test
{
    class TaskItemImpl : ITaskItem
    {
        private int sleepMilliseconds = 0;
        private string value = string.Empty;

        public TaskItemImpl(string name, int sleepMilliseconds, string value)
        {
            this.Name = name;
            this.sleepMilliseconds = sleepMilliseconds;
            this.value = value;
        }

        public string Name
        {
            get;
            set;
        }

        public void Execute()
        {
            Log.DebugFormat("task [{0}] is Executing.", this.Name);

            Thread.Sleep(this.sleepMilliseconds);

            KeyValuePool.Set(this.GetHashCode().ToString(), new Result() { TimeStamp = DateTime.Now, Value = this.value });

            Log.DebugFormat("task [{0}] is completed.", this.Name);
        }

        public void Complete()
        {
            if (this.TaskCompleted != null)
            {
                this.TaskCompleted(this, null);
            }
        }

        public event Src.EventHandler TaskCompleted;

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
