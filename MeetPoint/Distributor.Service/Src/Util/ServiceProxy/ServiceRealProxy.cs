using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;

namespace Distributor.Service.Src.Util.ServiceProxy
{
    internal class ServiceRealProxy<T> : RealProxy
    {
        private string endpointName = string.Empty;
        private InstanceContext callbackInstance = null;
        private string Uri = string.Empty;

        public ServiceRealProxy(string endpointName)
            : this(null, endpointName, string.Empty)
        {
        }

        public ServiceRealProxy(string endpointName, string Uri)
            : this(null, endpointName, Uri)
        {
        }

        public ServiceRealProxy(InstanceContext callbackInstance, string endpointName)
            : this(callbackInstance, endpointName, string.Empty)
        {
        }

        public ServiceRealProxy(InstanceContext callbackInstance, string endpointName, string Uri)
            : base(typeof(T))
        {
            Guard.NotNullOrEmpty(() => endpointName);

            this.endpointName = endpointName;
            this.callbackInstance = callbackInstance;
            this.Uri = Uri;
        }

        public override IMessage Invoke(IMessage msg)
        {
            ChannelFactory<T> channelFactory = ChannelFactoryCreator.Create<T>(this.callbackInstance, this.endpointName);

            T channel = default(T);

            if (string.IsNullOrEmpty(this.Uri))
            {
                channel = channelFactory.CreateChannel();
            }
            else
            {
                channel = channelFactory.CreateChannel(new EndpointAddress(this.Uri));
            }

            IMethodCallMessage methodCall = (IMethodCallMessage)msg;

            IMethodReturnMessage methodReturn = null;

            object[] copiedArgs = Array.CreateInstance(typeof(object), methodCall.Args.Length) as object[];

            methodCall.Args.CopyTo(copiedArgs, 0);

            try
            {
                object returnValue = methodCall.MethodBase.Invoke(channel, copiedArgs);

                methodReturn = new ReturnMessage(returnValue, copiedArgs, copiedArgs.Length, methodCall.LogicalCallContext, methodCall);

                // if the channel is duplex, we should NOT close it after invocation.
                if (this.callbackInstance == null)
                {
                    (channel as ICommunicationObject).Close();
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is CommunicationException || ex.InnerException is TimeoutException)
                {
                    (channel as ICommunicationObject).Abort();
                }

                if (ex.InnerException != null)
                {
                    methodReturn = new ReturnMessage(ex.InnerException, methodCall);
                }
                else
                {
                    methodReturn = new ReturnMessage(ex, methodCall);
                }
            }

            return methodReturn;
        }
    }
}
