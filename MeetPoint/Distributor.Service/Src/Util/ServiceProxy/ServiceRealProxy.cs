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
        private static Dictionary<string, T> DUPLEX_CHANNEL_MAP = new Dictionary<string, T>();

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

        public static T GetOpenedDuplexChannel(string endpointName)
        {
            lock (DUPLEX_CHANNEL_MAP)
            {
                if (DUPLEX_CHANNEL_MAP.ContainsKey(endpointName)
                    && ((DUPLEX_CHANNEL_MAP[endpointName] as ICommunicationObject).State == CommunicationState.Opened))
                {
                    return DUPLEX_CHANNEL_MAP[endpointName];
                }

                return default(T);
            }
        }

        public override IMessage Invoke(IMessage msg)
        {
            T channel = default(T);

            lock (DUPLEX_CHANNEL_MAP)
            {
<<<<<<< HEAD
                channel = channelFactory.CreateChannel();
            }
            else
            {
                // http://blogs.msdn.com/b/tiche/archive/2011/07/13/wcf-on-intranet-with-windows-authentication-kerberos-or-ntlm-part-1.aspx
                channel = channelFactory.CreateChannel(
                    new EndpointAddress(new Uri(this.Uri), EndpointIdentity.CreateSpnIdentity("MySystem/Service1")));
            }
=======
                // check the duplex channel
                if (DUPLEX_CHANNEL_MAP.ContainsKey(this.endpointName)
                    && ((DUPLEX_CHANNEL_MAP[this.endpointName] as ICommunicationObject).State == CommunicationState.Opened))
                {
                    channel = DUPLEX_CHANNEL_MAP[this.endpointName];
                }
                else
                {
                    // create channel
                    ChannelFactory<T> channelFactory = ChannelFactoryCreator.Create<T>(this.callbackInstance, this.endpointName);
                    if (string.IsNullOrEmpty(this.Uri))
                    {
                        channel = channelFactory.CreateChannel();
                    }
                    else
                    {
                        channel = channelFactory.CreateChannel(new EndpointAddress(this.Uri));
                    }

                    // cache duplex channel
                    if (this.callbackInstance != null)
                    {
                        DUPLEX_CHANNEL_MAP[this.endpointName] = channel;
                    }
                }
            } // lock (this.duplexChannelMap)
>>>>>>> remotes/a23126-04/master

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
