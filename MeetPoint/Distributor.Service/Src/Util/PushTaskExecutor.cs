using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Distributor.Service.Src.Contract;
using System.Threading;
using log4net;
using System.Reflection;

namespace Distributor.Service.Src.Util
{
    public static class PushTaskExecutor
    {
        static BackgroundWorker BgWorker = new BackgroundWorker();
        static List<ICallbackPushTask> PushTaskList = new List<ICallbackPushTask>();

        static Dictionary<string, ICallbackPushTask> CALLBACK_MAP = new Dictionary<string, ICallbackPushTask>();

        static PushTaskExecutor()
        {
            BgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            BgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgWorker_RunWorkerCompleted);
        }

        static void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("BgWorker_RunWorkerCompleted");
            Console.WriteLine(e.Error.Message);
        }

        static void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                Console.WriteLine("Push task executing ...");

                lock (PushTaskList)
                {
                    foreach (ICallbackPushTask nextTask in PushTaskList)
                    {
                        Console.WriteLine("About send to client ...");
                        try
                        {
                            nextTask.Display("Send result from server.");
                        }
                        catch (Exception exp)
                        {
                            Console.WriteLine(exp.Message);
                        }
                    }
                }

                Thread.Sleep(2000);

            } while (true);
        }

        public static void Start()
        {
            BgWorker.RunWorkerAsync();
        }

        public static void AddTask(ICallbackPushTask pushTask)
        {
            lock (PushTaskList)
            {
                PushTaskList.Add(pushTask);
            }
        }

        public static void RemoveTask(ICallbackPushTask pushTask)
        {
            lock (PushTaskList)
            {
                PushTaskList.Remove(pushTask);
            }
        }

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
