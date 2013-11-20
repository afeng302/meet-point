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
            this.IsDistributable = false;
        }

        public string Name
        {
            get;
            set;
        }

        public string ID { get; set; }

        public bool IsDistributable { get; private set; }

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


        public ValueType[] BoxFlyParams()
        {
            throw new NotImplementedException();
        }

        public void UnboxFlyParams(ValueType[] flyParams)
        {
            throw new NotImplementedException();
        }


        public Distributor.Service.Src.Contract.TaskStatus Status
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public string[] BoxFlyFiles()
        {
            throw new NotImplementedException();
        }

        public void UnboxFlyFiles(string[] flyFiles)
        {
            throw new NotImplementedException();
        }
    }
}
