using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelTaskScheduler.src
{
    public class TaskContainer
    {
        Queue<ParallelTaskNode> taskNodeQueue = new Queue<ParallelTaskNode>();

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
    }
}
