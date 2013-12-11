using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelTaskScheduler.Src.TaskFactory
{
    public interface ITaskFactory
    {
        ITaskItem CreateTask(string className);
    }
}
