using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Distributor.Service.Src.Service;
using Distributor.Service.Src.Util;
using System.Reflection;
using System.IO;

namespace Distributor.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            PushTaskExecutor.Start();

            using (ServiceHost loginHost = new ServiceHost(typeof(LoginService)),
                fileRepoHost = new ServiceHost(typeof(FileRepositoryService)))
            {
                loginHost.Open();
                Console.WriteLine("Login service has begun to listen ");

                fileRepoHost.Open();
                //(fileRepoHost.SingletonInstance as FileRepositoryService).RepositoryDirectory 
                //    = Path.Combine(Assembly.GetExecutingAssembly().Location, "file_repo_service");
                Console.WriteLine("File Repository service has begun to listen ");

                Console.Read();
            }
        }
    }
}
