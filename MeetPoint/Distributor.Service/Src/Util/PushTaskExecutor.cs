using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Distributor.Service.Src.Contract;
using System.Threading;

namespace Distributor.Service.Src.Util
{
    public static class PushTaskExecutor
    {
        static BackgroundWorker BgWorker = new BackgroundWorker();
        static List<ICallbackPushTask> PushTaskList = new List<ICallbackPushTask>();

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
    }
}
