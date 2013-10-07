using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MeetPoint.Src;
using System.Threading;
using System.ComponentModel;
using log4net;
using System.Reflection;

namespace ParallelTaskScheduler.Src
{
    public static class ParallelTaskScheduler
    {
        public static void  Schedule(TaskContainer container)
        {
            ParallelTaskNode currTaskNode = null;

            currTaskNode = container.DeQueueTaskNode();
            do
            {
                // assign meet point
                IMeetPoint meetPoint = AssignMeetPoint(currTaskNode);

                // lanuch tasks parallelly
                LanuchTasks(currTaskNode);

                // wait for all tasks completed
                meetPoint.PostCondArrive(null);
                Log.DebugFormat("taskNode with [{0}] is completed.",
                    currTaskNode.Tasks.Length > 0 ? currTaskNode.Tasks[0].Name : string.Empty);

                // move to next task node
                currTaskNode = container.DeQueueTaskNode();
            } while (currTaskNode != null);
        }

        private static IMeetPoint AssignMeetPoint(ParallelTaskNode taskNode)
        {
            if (taskNode == null)
            {
                return null;
            }

            string pointID = Guid.NewGuid().ToString();
            bool createdNew = false;

            // the only post condition is created for task scheduller
            IMeetPoint meetPoint = MeetPointFactory.Create(pointID, taskNode.TaskCount, 1,
                Timeout.Infinite, out createdNew);

            foreach (ITaskItem nextTask in taskNode.Tasks)
            {
                nextTask.TaskCompleted += (sender, e) =>
                    {
                        meetPoint.PreCondArrive(null);
                    };
            }

            return meetPoint;
        }

        private static void LanuchTasks(ParallelTaskNode taskNode)
        {
            foreach (ITaskItem nextTask in taskNode.Tasks)
            {
                BackgroundWorker bgWorder = new BackgroundWorker();

                // use local variable in the anonynouse method or lambda expression
                // http://www.codeproject.com/Articles/15624/Inside-C-2-0-Anonymous-Methods
                ITaskItem localNextTask = nextTask;

                bgWorder.DoWork += (object sender, DoWorkEventArgs e) =>
                    {
                        localNextTask.Execute();
                    };

                bgWorder.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
                    {
                        localNextTask.Complete();
                    };

                bgWorder.RunWorkerAsync();

                Log.DebugFormat("task [{0}] scheduled.", localNextTask.Name);
            }
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
