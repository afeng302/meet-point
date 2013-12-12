using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Distributor.Service.Src.Service;
using Distributor.Service.Src.Util;
using System.Reflection;
using System.IO;
using log4net;
using Distributor.Service.Src.Contract;
using ParallelTaskScheduler.Src;
using Distributor.TestUtil.Task;
using ParallelTaskScheduler.Src.TaskFactory;

namespace Distributor.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            // ** test code **
            //PushTaskExecutor.Start();

            //
            // start service
            ServiceHost loginHost = new ServiceHost(typeof(LoginService));
            ServiceHost fileRepoHost = new ServiceHost(typeof(FileRepositoryService));
            ServiceHost taskScheduleHost = new ServiceHost(typeof(TaskScheduleService));

            loginHost.Open();
            Console.WriteLine("Login service has begun to listen ");
            Log.Info("Login service has begun to listen ");

            fileRepoHost.Open();
            //(fileRepoHost.SingletonInstance as FileRepositoryService).RepositoryDirectory 
            //    = Path.Combine(Assembly.GetExecutingAssembly().Location, "file_repo_service");
            Console.WriteLine("File Repository service has begun to listen ");
            Log.Info("File Repository service has begun to listen ");

            taskScheduleHost.Open();
            Console.WriteLine("Task Schedule service has begun to listen ");
            Log.Info("Task Schedule service has begun to listen ");
            
            //
            // connect client to service
            InstanceContext instanceContext = new InstanceContext(new CallbackPushTaskService());
            ILogin loginProxy = null;
            loginProxy = ServiceProxyFactory.Create<ILogin>(instanceContext, "LoginService", "net.tcp://localhost:1234/login");
            loginProxy.Login("client-from-host");

            //
            // init service factory
            ServiceFactory.ENFileRepoService = "FileRepositoryService";
            ServiceFactory.ENTaskScheduleService = "TaskScheduleService";

            //
            // init ParallelTaskSchedulerset local host name
            ParallelTaskScheduler.Src.ParallelTaskScheduler.LocalHostName = "client-from-host";
            ParallelTaskScheduler.Src.ParallelTaskScheduler.IsClusterMode = true;

            //
            // init task factory
            TaskFactoryRender.SetFactory(new TaskFactoryImpl());

            //
            // start remote task server
            RemoteTaskServer.Start();

            //
            // wait for end ...
            Console.ReadLine();
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
