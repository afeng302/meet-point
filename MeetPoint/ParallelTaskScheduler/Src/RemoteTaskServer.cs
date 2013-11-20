using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using log4net;
using System.Reflection;
using MeetPoint.Src;
using Distributor.Service.Src.Contract;
using System.IO;
using ParallelTaskScheduler.Util;

namespace ParallelTaskScheduler.Src
{
    public static class RemoteTaskServer
    {
        static Queue<string> RECV_QUEUE = new Queue<string>();
        static BackgroundWorker RECV_WORKER = new BackgroundWorker();
        static IMeetPoint RECV_MEET_POINT = null;

        static Queue<TransferTaskItem> SEND_QUEUE = new Queue<TransferTaskItem>();
        static BackgroundWorker SEND_WORKER = new BackgroundWorker();
        static IMeetPoint SEND_MEET_POINT = null;

        static RemoteTaskServer()
        {
            RECV_WORKER.WorkerSupportsCancellation = true;
            RECV_WORKER.WorkerReportsProgress = true;
            RECV_WORKER.DoWork += new DoWorkEventHandler(Recv_Worker_DoWork);
            RECV_WORKER.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Recv_Worker_RunWorkerCompleted);

            bool createdNew = false;
            RECV_MEET_POINT = MeetPointFactory.Create(Guid.NewGuid().ToString(), 1, 1, out createdNew);

            SEND_WORKER.WorkerSupportsCancellation = true;
            SEND_WORKER.WorkerReportsProgress = true;
            SEND_WORKER.DoWork += new DoWorkEventHandler(SEND_WORKER_DoWork);
            SEND_WORKER.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SEND_WORKER_RunWorkerCompleted);

        }

        public static string MasterNodeName { get; set; }

        public static void Start()
        {
            RECV_WORKER.RunWorkerAsync();
        }

        public static void Stop()
        {
            RECV_WORKER.CancelAsync();
        }

        public static void PendRecvTask(string taskID)
        {
            lock (RECV_QUEUE)
            {
                RECV_QUEUE.Enqueue(taskID);
            }

            RECV_MEET_POINT.PreCondArrive(null);
        }

        public static void PendSendTask(TransferTaskItem sendTaskItem)
        {
            lock (SEND_QUEUE)
            {
                SEND_QUEUE.Enqueue(sendTaskItem);
            }

            SEND_MEET_POINT.PreCondArrive(null);
        }

        static void Recv_Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                // terminated unexpected
                Log.ErrorFormat("recv worker terminated unexpected. {0}", e.Error);

                // start the background worker again
                Log.Info("recv worker was restarted again ...");
                RECV_WORKER.RunWorkerAsync();

                return;
            }

            Log.Info("recv worker existed.");
        }

        static void Recv_Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                RECV_MEET_POINT.PostCondArrive(null);

                string taskID = string.Empty;
                lock (RECV_QUEUE)
                {
                    if (RECV_QUEUE.Count > 0)
                    {
                        taskID = RECV_QUEUE.Dequeue();
                    }
                    else
                    {
                        RECV_MEET_POINT.Reset();
                        continue;
                    }
                }

                // get task from master
                TransferTaskItem taskItem = ServiceFactory.GetTaskService().PullTask(taskID);
                Log.InfoFormat("pulled task: [{0}]", taskID);

                // get files for the task
                if (taskItem.Files != null)
                {
                    foreach (string nextFile in taskItem.Files)
                    {
                        // get file
                        Stream srcStream = ServiceFactory.GetFileRepoService().GetFile(nextFile);
                        string filePath = Path.GetFullPath(Path.Combine(TransferHelper.LocalRootPath, nextFile));
                        using (FileStream destStream = new FileStream(filePath, FileMode.Create))
                        {
                            srcStream.CopyTo(destStream);
                        }
                        Log.InfoFormat("got file: [{0}]", nextFile);
                        // delete file
                        ServiceFactory.GetFileRepoService().DeleteFile(nextFile);
                        Log.InfoFormat("deleted file: [{0}]", nextFile);
                    }
                }

                // execute the task
                new RemoteTaskExecutor(taskItem).Run();
            } while (true);
        }

        static void SEND_WORKER_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                // terminated unexpected
                Log.ErrorFormat("send worker terminated unexpected. {0}", e.Error);

                // start the background worker again
                Log.Info("send worker was restarted again ...");
                SEND_WORKER.RunWorkerAsync();

                return;
            }

            Log.Info("send worker existed.");
        }

        static void SEND_WORKER_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                SEND_MEET_POINT.PostCondArrive(null);

                TransferTaskItem sendTask = null;
                lock (SEND_QUEUE)
                {
                    if (SEND_QUEUE.Count > 0)
                    {
                        sendTask = SEND_QUEUE.Dequeue();
                    }
                    else
                    {
                        SEND_MEET_POINT.Reset();
                        continue;
                    }
                }

                // send task to master node
                ServiceFactory.GetTaskService().PushTask(sendTask);
                Log.InfoFormat("pushed task: [{0}]", sendTask.ID);

                // send files for the task
                if (sendTask.Files != null)
                {
                    foreach (string nextFile in sendTask.Files)
                    {
                        string filePath = Path.GetFullPath(Path.Combine(TransferHelper.LocalRootPath, nextFile));
                        using (FileStream uploadStream = new FileStream(filePath, FileMode.Open))
                        {
                            ServiceFactory.GetFileRepoService().PutFile(new FileUploadMessage()
                                {
                                    DataStream = uploadStream,
                                    VirtualPath = nextFile,
                                });
                        }
                        Log.InfoFormat("put file: [{0}]", nextFile);
                    }
                }
            } while (true);
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
