using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using Distributor.Service.Src.Util;

namespace ParallelTaskScheduler.Src
{
    public static class ServiceFactory
    {
        public static string ENTaskScheduleService { get; set; }

        public static string ENFileRepoService { get; set; }

        public static ITaskScheduleService GetTaskService()
        {
            return ServiceProxyFactory.Create<ITaskScheduleService>(ENTaskScheduleService);
        }

        public static IFileRepositoryService GetFileRepoService()
        {
            return ServiceProxyFactory.Create<IFileRepositoryService>(ENFileRepoService);
        }
    }
}
