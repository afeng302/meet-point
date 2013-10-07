using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Distributor.Service.Src.Util
{
    /// <summary>
    /// Use generic delegate implement AOP
    /// </summary>
    public class ServiceInvoker
    {
        private static Dictionary<string, ChannelFactory> channelFactories = new Dictionary<string, ChannelFactory>();

        private static object syncHelper = new object();

        private static ChannelFactory<TChannel> GetChannelFactory<TChannel>(string endpointConfigurationName)
        {
            ChannelFactory<TChannel> channelFactory = null;

            Guard.NotNullOrEmpty(() => endpointConfigurationName);

            if (channelFactories.ContainsKey(endpointConfigurationName))
            {
                channelFactory = channelFactories[endpointConfigurationName] as ChannelFactory<TChannel>;
            }

            if (null == channelFactory)
            {
                channelFactory = new ChannelFactory<TChannel>(endpointConfigurationName);
                lock (syncHelper)
                {
                    channelFactories[endpointConfigurationName] = channelFactory;
                }
            }

            return channelFactory;
        }

        public static void Invoke<TChannel>(Action<TChannel> action, TChannel proxy)
        {
            ICommunicationObject channel = proxy as ICommunicationObject;
            if (null == channel)
            {
                throw new ArgumentException("The proxy is not a valid channel implementing the ICommunicationObject interface", "proxy");
            }

            try
            {
                action(proxy);
            }
            catch (TimeoutException)
            {
                channel.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                channel.Abort();
                throw;
            }
            finally
            {
                channel.Close();
            }
        }

        public static TResult Invoke<TChannel, TResult>(Func<TChannel, TResult> function, TChannel proxy)
        {
            ICommunicationObject channel = proxy as ICommunicationObject;
            if (null == channel)
            {
                throw new ArgumentException("The proxy is not a valid channel implementing the ICommunicationObject interface", "proxy");
            }
            try
            {
                return function(proxy);
            }
            catch (TimeoutException)
            {
                channel.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                channel.Abort();
                throw;
            }
            finally
            {
                channel.Close();
            }
        }

        public static void Invoke<TChannel>(Action<TChannel> action, string endpointConfigurationName)
        {
            Guard.NotNullOrEmpty(() => endpointConfigurationName);
            //Guard.ArgumentNotNullOrEmpty(endpointConfigurationName, "endpointConfigurationName");
            Invoke<TChannel>(action, GetChannelFactory<TChannel>(endpointConfigurationName).CreateChannel());
        }

        public static TResult Invoke<TChannel, TResult>(Func<TChannel, TResult> function, string endpointConfigurationName)
        {
            Guard.NotNullOrEmpty(() => endpointConfigurationName);
            //Guard.ArgumentNotNullOrEmpty(endpointConfigurationName, "endpointConfigurationName");
            return Invoke<TChannel, TResult>(function, GetChannelFactory<TChannel>(endpointConfigurationName).CreateChannel());
        }
    }
}
