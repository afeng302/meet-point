using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using System.ServiceModel;
using Distributor.Service.Src.Util;
using Distributor.Service.Src.Service;
using System.IO;
using System.Diagnostics;
using ParallelTaskScheduler.Src;
using Distributor.TestUtil.Task;
using ParallelTaskScheduler.Src.TaskFactory;

namespace Distributor.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            InstanceContext instanceContext = new InstanceContext(new CallbackPushTaskService());
            ILogin loginProxy = null; 
            //CommunicationState state;
            //MeetPoint.Src.
            //using (DuplexChannelFactory<ILogin> channelFactory = new DuplexChannelFactory<ILogin>(instanceContext, "login"))
            //{
            //    loginProxy = channelFactory.CreateChannel();
            //    loginProxy.Login("test login");

            //    //(loginProxy as ICommunicationObject).Close();
            //    state = (loginProxy as ICommunicationObject).State;
            //    Console.ReadLine();
            //}

            //state = (loginProxy as ICommunicationObject).State;

            //ILogin loginProxy = ServiceProxyFactory.Create<ILogin>("login");

            //
            // Login
            loginProxy = ServiceProxyFactory.Create<ILogin>(instanceContext, "LoginService", "net.tcp://localhost:1234/login");
            loginProxy.Login("client-01");

            //
            // init service factory
            ServiceFactory.ENFileRepoService = "FileRepositoryService";
            ServiceFactory.ENTaskScheduleService = "TaskScheduleService";
            
            //
            // set local host name
            ParallelTaskScheduler.Src.ParallelTaskScheduler.LocalHostName = "client-01";

            //
            // init task factory
            TaskFactoryRender.SetFactory(new TaskFactoryImpl());

            //
            // start remote task server
            RemoteTaskServer.Start();

            //
            // schedule task
            TaskContainer container = new TaskContainer().AddOrdered(new ReadRadio())
                .AddOrdered(new PrepareData()).AddOrdered(new WriteRadio());
            ParallelTaskScheduler.Src.ParallelTaskScheduler.Schedule(container);
            
            Console.ReadLine();

            //ILogin loginProxy = new ChannelFactory<ILogin>("login").CreateChannel();


            //loginProxy.Login("test login");
        }

        static void FileRepoServiceTest()
        {
            IFileRepositoryService repoService = ServiceProxyFactory.Create<IFileRepositoryService>("FileRepositoryService");

            using (FileStream uploadStream1 = new FileStream("D:/test_data/iTM_Help_en.chm", FileMode.Open),
                uploadStream2 = new FileStream("D:/test_data/rpk/MR5.13.3_NonE2E_Clear_FW7893_R01011100.new.rpk", FileMode.Open))
            {
                // put file
                repoService.PutFile(new FileUploadMessage()
                {
                    VirtualPath = "./help/iTM_Help_en.chm",
                    DataStream = uploadStream1
                });

                // get file file info
                StorageFileInfo[] infos = repoService.GetFileInfo("help/iTM_Help_en.chm");
                Debug.Assert(infos.Length == 1);
                Debug.Assert(infos[0].VirtualPath == "help/iTM_Help_en.chm");
                Debug.Assert(infos[0].Size == uploadStream1.Length);

                // get file
                Stream s = repoService.GetFile("help/iTM_Help_en.chm");
                using (FileStream outputStream = new FileStream("./iTM_Help_en.chm", FileMode.Create))
                {
                    s.CopyTo(outputStream);
                    Debug.Assert(outputStream.Length == infos[0].Size);
                }
                File.Delete("./iTM_Help_en.chm");

                // delete file
                repoService.DeleteFile("help/iTM_Help_en.chm");
                infos = repoService.GetFileInfo("help/iTM_Help_en.chm");
                Debug.Assert((infos == null) || (infos.Length == 0));

                // put file
                repoService.PutFile(new FileUploadMessage()
                {
                    VirtualPath = "./rpk/MR5.13.3_NonE2E_Clear_FW7893_R01011100.new.rpk",
                    DataStream = uploadStream2
                });
                // delete file
                repoService.DeleteFile("./rpk/MR5.13.3_NonE2E_Clear_FW7893_R01011100.new.rpk");
            }
        }
    }
}
