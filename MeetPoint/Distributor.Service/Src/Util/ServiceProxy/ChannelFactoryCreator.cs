using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ServiceModel;

namespace Distributor.Service.Src.Util.ServiceProxy
{
    internal static class ChannelFactoryCreator
    {
        private static Hashtable channelFactories = new Hashtable();

        public static ChannelFactory<T> Create<T>(string endpointName)
        {
            return Create<T>(null, endpointName);
        }

        public static ChannelFactory<T> Create<T>(InstanceContext callbackInstance, string endpointName)
        {
            ChannelFactory<T> channelFactory = null;

            // guard end point name is not empty
            Guard.NotNullOrEmpty(() => endpointName);

            // reuse existing factory
            if (channelFactories.ContainsKey(endpointName))
            {
                channelFactory = channelFactories[endpointName] as ChannelFactory<T>;
            }

            // create new channel factory
            if (channelFactory == null)
            {
                if (callbackInstance != null)
                {
                    channelFactory = new DuplexChannelFactory<T>(callbackInstance, endpointName);
                }
                else
                {
                    channelFactory = new ChannelFactory<T>(endpointName);
                }
                
                lock (channelFactories.SyncRoot)
                {
                    channelFactories[endpointName] = channelFactory;
                }
            }

            return channelFactory;
        }
    }
}
