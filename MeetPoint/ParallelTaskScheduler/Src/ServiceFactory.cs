using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using Distributor.Service.Src.Util;

namespace ParallelTaskScheduler.Src
{
    static class ServiceFactory
    {
        public static string TaskServiceEndpointName { get; set; }

        public static string FileRepoServiceEndpointName { get; set; }

        public static ITaskScheduleService GetTaskService()
        {
            return ServiceProxyFactory.Create<ITaskScheduleService>(TaskServiceEndpointName);
        }

        public static IFileRepositoryService GetFileRepoService()
        {
            return ServiceProxyFactory.Create<IFileRepositoryService>(FileRepoServiceEndpointName);
        }
    }
}
