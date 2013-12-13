using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using System.ServiceModel;
using Distributor.Service.Src.Util;
using log4net;
using System.Reflection;
using Distributor.Service.Src.Manager;

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
            Log.InfoFormat("Login: [{0}]", this.clientHostName);

            this.callback = OperationContext.Current.GetCallbackChannel<ICallbackPushTask>();

            // add callback channel
            CallbackChannelManager.AddCallbackChannel(this.clientHostName, this.callback);

            // add payload
            PayloadManager.AddNode(this.clientHostName);

            // callback.Display("Send result from server.");
            // *** test code ***
            PushTaskExecutor.AddTask(callback);

            (callback as ICommunicationObject).Closed += new EventHandler(LoginService_Closed);
            (callback as ICommunicationObject).Faulted += new EventHandler(LoginService_Faulted);
        }

        public void Logout()
        {
            // close callback channel
            CallbackChannelManager.CloseCallbackChannel(this.clientHostName);

            // remove callback channel
            CallbackChannelManager.RemoveCallbackChannel(this.clientHostName);

            Log.InfoFormat("Logout: [{0}]", this.clientHostName);
        }


        public void Heartbeat()
        {
            Log.InfoFormat("Heartbeat from [{0}] ...", this.clientHostName);
        }

        void LoginService_Faulted(object sender, EventArgs e)
        {
            Console.WriteLine(clientHostName + " Faulted !!!");
            PushTaskExecutor.RemoveTask(this.callback);

            CallbackChannelManager.CloseCallbackChannel(this.clientHostName);
            CallbackChannelManager.RemoveCallbackChannel(this.clientHostName);
            PayloadManager.RemoveNode(this.clientHostName);

            Log.ErrorFormat("channel to [{0}] is faulted.", this.clientHostName);
        }

        void LoginService_Closed(object sender, EventArgs e)
        {
            Console.WriteLine(clientHostName + " Closed !!!");
            PushTaskExecutor.RemoveTask(this.callback);

            CallbackChannelManager.CloseCallbackChannel(this.clientHostName);
            CallbackChannelManager.RemoveCallbackChannel(this.clientHostName);
            PayloadManager.RemoveNode(this.clientHostName);

            Log.InfoFormat("channel to [{0}] is closed.", this.clientHostName);
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
