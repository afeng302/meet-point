using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelTaskScheduler.src
{
    public class ParallelTaskNode
    {
        List<ITaskItem> taskList = new List<ITaskItem>();

        public void Add(ITaskItem taskItem)
        {
            lock (this.taskList)
            {
                this.taskList.Add(taskItem);
            }
        }

        public ITaskItem[] Tasks
        {
            get
            {
                lock (this.taskList)
                {
                    return this.taskList.ToArray();
                }
            }
        }

        public int TaskCount
        {
            get
            {
                lock (this.taskList)
                {
                    return this.taskList.Count;
                }
            }
        }
    }
}
