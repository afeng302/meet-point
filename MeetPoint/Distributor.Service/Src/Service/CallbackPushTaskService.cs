using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;

namespace Distributor.Service.Src.Service
{
    public class CallbackPushTaskService : ICallbackPushTask
    {
        public void Display(string result)
        {
            Console.WriteLine(result);
        }
    }
}
