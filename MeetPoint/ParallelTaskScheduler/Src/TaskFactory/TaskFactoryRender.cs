using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelTaskScheduler.Src.TaskFactory
{
    public static class TaskFactoryRender
    {
        static ITaskFactory FACTORY = null;

        public static void SetFactory(ITaskFactory factoryImpl)
        {
            FACTORY = factoryImpl;
        }

        public static ITaskFactory GetFactory()
        {
            return FACTORY;
        }
    }
}
