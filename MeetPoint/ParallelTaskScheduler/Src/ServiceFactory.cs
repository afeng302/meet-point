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
        public static string ENLoginService { get; set; }

        public static string ENTaskScheduleService { get; set; }

        public static string ENFileRepoService { get; set; }

        public static string MastNodeName { get; set; }

        public static ITaskScheduleService GetTaskService()
        {
            if (string.IsNullOrEmpty(MastNodeName))
            {
                return ServiceProxyFactory.Create<ITaskScheduleService>(ENTaskScheduleService);
            }

            string uri = string.Format(@"net.tcp://{0}:1234/TaskSchedule", MastNodeName);

            return ServiceProxyFactory.Create<ITaskScheduleService>(ENTaskScheduleService, uri);
        }

        public static IFileRepositoryService GetFileRepoService()
        {
            if (string.IsNullOrEmpty(MastNodeName))
            {
                return ServiceProxyFactory.Create<IFileRepositoryService>(ENFileRepoService);
            }

            string uri = string.Format(@"net.tcp://{0}:1234/FileRepository", MastNodeName);

            return ServiceProxyFactory.Create<IFileRepositoryService>(ENFileRepoService, uri);
        }
    }
}
