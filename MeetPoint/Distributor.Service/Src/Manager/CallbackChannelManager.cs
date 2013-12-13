using System.Collections.Generic;
using System.Reflection;
using Distributor.Service.Src.Contract;
using log4net;
using Distributor.Service.Src.Util;
using System.ServiceModel;

namespace Distributor.Service.Src.Manager
{
    static class CallbackChannelManager
    {
        static Dictionary<string, ICallbackPushTask> CALLBACK_MAP = new Dictionary<string, ICallbackPushTask>();

        public static void AddCallbackChannel(string clientHostName, ICallbackPushTask pushTask)
        {
            Guard.ArgumentNotNullOrEmpty(clientHostName, "clientHostName");
            Guard.ArgumentNotNull(pushTask, "pushTask");

            lock (CALLBACK_MAP)
            {
                if (CALLBACK_MAP.ContainsKey(clientHostName))
                {
                    Log.ErrorFormat("duplicate callback channel [{0}]", clientHostName);
                    return;
                }

                CALLBACK_MAP[clientHostName] = pushTask;
                Log.InfoFormat("callback channel is added [{0}]", clientHostName);
            }
        }

        public static void RemoveCallbackChannel(string clientHostName)
        {
            Guard.ArgumentNotNullOrEmpty(clientHostName, "clientHostName");

            lock (CALLBACK_MAP)
            {
                if (!CALLBACK_MAP.Remove(clientHostName))
                {
                    Log.ErrorFormat("callback channel not found for removing [{0}]", clientHostName);
                }
                else
                {
                    Log.InfoFormat("callback channel is removed [{0}]", clientHostName);
                }
            }
        }

        public static void CloseCallbackChannel(string clientHostName)
        {
            Guard.ArgumentNotNullOrEmpty(clientHostName, "clientHostName");

            lock (CALLBACK_MAP)
            {
                if (!CALLBACK_MAP.ContainsKey(clientHostName))
                {
                    Log.ErrorFormat("callback channel not found for close [{0}]", clientHostName);
                    return;
                }

                ((ICommunicationObject)CALLBACK_MAP[clientHostName]).Close();
            }
        }

        public static ICallbackPushTask GetCallbackChannel(string clientHostName)
        {
            Guard.ArgumentNotNullOrEmpty(clientHostName, "clientHostName");

            lock (CALLBACK_MAP)
            {
                if (!CALLBACK_MAP.ContainsKey(clientHostName))
                {
                    Log.ErrorFormat("callback channel not found for getting [{0}]", clientHostName);
                    return null;
                }

                return CALLBACK_MAP[clientHostName];
            }
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
