using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using ParallelTaskScheduler.Src;

namespace Distributor.Client
{
    public class CallbackPushTaskService : ICallbackPushTask
    {
        public void Display(string result)
        {
            Console.WriteLine("Reply from server: " + result);

            RemoteTaskServer.PendRecvTask(string.Empty);
        }
    }
}
