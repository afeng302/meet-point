using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Util;
using log4net;
using System.Reflection;

namespace Distributor.Service.Src.Manager
{
    static class PayloadManager
    {
        static Dictionary<string, int> PAYLOAD_MAP = new Dictionary<string, int>();

        public static void AddNode(string hostName)
        {
            Guard.ArgumentNotNullOrEmpty(hostName, "hostName");

            lock (PAYLOAD_MAP)
            {
                if (PAYLOAD_MAP.ContainsKey(hostName))
                {
                    Log.ErrorFormat("host[{0}] is already exist in payload map", hostName);
                    return;
                }

                PAYLOAD_MAP[hostName] = 0;
            }

            Log.InfoFormat("[{0}] was added into payload map", hostName);
        }

        public static void RemoveNode(string hostName)
        {
            Guard.ArgumentNotNullOrEmpty(hostName, "hostName");

            lock (PAYLOAD_MAP)
            {
                if (!PAYLOAD_MAP.Remove(hostName))
                {
                    Log.ErrorFormat("host[{0}] cannot be found in payload map", hostName);
                }
            }

            Log.InfoFormat("[{0}] was removed from payload map", hostName);
        }

        public static void IncreasePayload(string hostName)
        {
            Guard.ArgumentNotNullOrEmpty(hostName, "hostName");

            lock (PAYLOAD_MAP)
            {
                if (!PAYLOAD_MAP.ContainsKey(hostName))
                {
                    Log.ErrorFormat("host[{0}] cannot be found to increase payload", hostName);
                    return;
                }

                PAYLOAD_MAP[hostName]++;
            }

            Log.InfoFormat("payload on [{0}] was increased", hostName);
        }

        public static void ReducePayload(string hostName)
        {
            Guard.ArgumentNotNullOrEmpty(hostName, "hostName");

            lock (PAYLOAD_MAP)
            {
                if (!PAYLOAD_MAP.ContainsKey(hostName))
                {
                    Log.ErrorFormat("host[{0}] cannot be found to reduce payload", hostName);
                    return;
                }

                PAYLOAD_MAP[hostName]--;

                if (PAYLOAD_MAP[hostName] < 0)
                {
                    Log.ErrorFormat("invalid payload [{0}] for host [{1}]", PAYLOAD_MAP[hostName], hostName);

                    PAYLOAD_MAP[hostName] = 0;
                }
            }

            Log.InfoFormat("payload on [{0}] was reduced", hostName);
        }

        public static string GetRelaxedNode()
        {
            int minPayload = int.MaxValue;
            string relaxedHost = string.Empty;

            lock (PAYLOAD_MAP)
            {
                if (PAYLOAD_MAP.Count == 0)
                {
                    Log.ErrorFormat("Empty payload map");
                    return string.Empty;
                }

                foreach (string nextHost in PAYLOAD_MAP.Keys)
                {
                    if (PAYLOAD_MAP[nextHost] < minPayload)
                    {
                        minPayload = PAYLOAD_MAP[nextHost];
                        relaxedHost = nextHost;
                    }
                }
            }

            return relaxedHost;
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
