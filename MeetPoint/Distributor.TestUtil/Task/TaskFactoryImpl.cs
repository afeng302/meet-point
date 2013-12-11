using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ParallelTaskScheduler.Src;
using ParallelTaskScheduler.Src.TaskFactory;

namespace Distributor.TestUtil.Task
{
    public class TaskFactoryImpl : ITaskFactory
    {
        public ITaskItem CreateTask(string className)
        {
            return Assembly.GetExecutingAssembly().CreateInstance(className) as ITaskItem;
        }
    }
}
