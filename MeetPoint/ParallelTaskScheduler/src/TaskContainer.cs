using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using Distributor.Service.Src.Util;

namespace ParallelTaskScheduler.Src
{
    public class TaskContainer
    {
        Queue<ParallelTaskNode> taskNodeQueue = new Queue<ParallelTaskNode>();
        Dictionary<string, object> taskContextMap = new Dictionary<string, object>();

        public TaskContainer AddParallel(ITaskItem taskItem)
        {
            lock (this.taskNodeQueue)
            {
                // add the first node into queue
                if (this.taskNodeQueue.Count == 0)
                {
                    this.taskNodeQueue.Enqueue(new ParallelTaskNode());
                }

                // add the task into last node
                this.taskNodeQueue.Last().Add(taskItem);
                taskItem.OwnContainer = this;
            }

            return this;
        }

        public TaskContainer AddOrdered(ITaskItem taskItem)
        {
            lock (this.taskNodeQueue)
            {
                // always add a new node into the queue
                this.taskNodeQueue.Enqueue(new ParallelTaskNode());

                // add the task into last node
                this.taskNodeQueue.Last().Add(taskItem);
                taskItem.OwnContainer = this;
            }

            return this;
        }

        public ParallelTaskNode DeQueueTaskNode()
        {
            lock (this.taskNodeQueue)
            {
                if (this.taskNodeQueue.Count == 0)
                {
                    return null;
                }

                return this.taskNodeQueue.Dequeue();
            }
        }

        public void SetTaskContext(string taskName, object context)
        {
            Guard.ArgumentNotNullOrEmpty(taskName, "taskName");

            lock (this.taskContextMap)
            {
                if (this.taskContextMap.ContainsKey(taskName))
                {
                    Log.ErrorFormat("task context already exist - task name [{0}]", taskName);
                    return;
                }

                this.taskContextMap[taskName] = context;
            }
        }

        public object GetTaskContext(string taskName)
        {
            Guard.ArgumentNotNullOrEmpty(taskName, "taskName");

            lock (this.taskContextMap)
            {
                if (!this.taskContextMap.ContainsKey(taskName))
                {
                    Log.ErrorFormat("task context not found - task name[{0}]", taskName);
                    return null;
                }

                return this.taskContextMap[taskName];
            }
        }


        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
