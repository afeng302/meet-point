using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using System.ServiceModel;
using Distributor.Service.Src.Util;

namespace Distributor.Service.Src.Service
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class LoginService : ILogin
    {
        string clientHostName = string.Empty;
        ICallbackPushTask callback = null;

        public void Login(string clientHostName)
        {
            this.clientHostName = clientHostName;

            Console.WriteLine("Login: " + clientHostName);

            this.callback = OperationContext.Current.GetCallbackChannel<ICallbackPushTask>();
            //callback.Display("Send result from server.");
            PushTaskExecutor.AddTask(callback);

            (callback as ICommunicationObject).Closed += new EventHandler(LoginService_Closed);

            (callback as ICommunicationObject).Faulted += new EventHandler(LoginService_Faulted);
        }

        void LoginService_Faulted(object sender, EventArgs e)
        {
            Console.WriteLine(clientHostName + " Faulted !!!");
            PushTaskExecutor.RemoveTask(this.callback);
        }

        void LoginService_Closed(object sender, EventArgs e)
        {
            Console.WriteLine(clientHostName + " Closed !!!");
            PushTaskExecutor.RemoveTask(this.callback);
        }
    }
}
