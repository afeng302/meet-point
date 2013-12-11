using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParallelTaskScheduler.Src;
using System.IO;
using System.Threading;
using log4net;
using System.Reflection;
using Distributor.Service.Src.Contract;

namespace Distributor.TestUtil.Task
{
    public class PrepareData : ITaskItem
    {
        public string Name
        {
            get
            {
                return "Prepare-Data";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string taskID = string.Empty;

        public string ID
        {
            get
            {
                if (string.IsNullOrEmpty(this.taskID))
                {
                    this.taskID = Guid.NewGuid().ToString();
                }
                return this.taskID;
            }
            set
            {
                this.taskID = value;
            }
        }

        public bool IsDistributable
        {
            get { return true; }
        }

        public Service.Src.Contract.TaskStatus Status
        {
            get;
            set;
        }

        public TaskExecuteType ExecuteType
        {
            get;
            set;
        }

        public ValueType[] BoxFlyParams()
        {
            return null;
        }

        public void UnboxFlyParams(ValueType[] flyParams)
        {
            // do nothing
        }

        public string[] BoxFlyFiles()
        {
            if (this.Status == Service.Src.Contract.TaskStatus.Ready)
            {
                // request
                return new string[1] { (string)this.OwnContainer.GetTaskContext("Read-Radio") };
            }
            else if (this.Status == Service.Src.Contract.TaskStatus.Completed)
            {
                // response
                return new string[1] { this.OutputDataLocalPath };
            }

            // execute failed, do nothing
            return null;
        }

        public void UnboxFlyFiles(string[] flyFiles)
        {
            if (this.Status == Service.Src.Contract.TaskStatus.Ready)
            {
                // request
                this.RadioDataPath = flyFiles[0];
            }
            else if (this.Status == Service.Src.Contract.TaskStatus.Completed)
            {
                // response
                this.OutputDataFlyPath = flyFiles[0];
            }
        }

        public void Execute()
        {
            Log.InfoFormat("task [{0}]/[{1}] is executing ...", this.Name, this.ID);

            // prepare the radio data
            if (this.OwnContainer != null)
            {
                // locally, get the radio data from task container
                this.RadioDataPath = (string)this.OwnContainer.GetTaskContext("Read-Radio");
            }
            else
            {
                // on fly
                // the radio data should be set in unbox process
            }

            using (StreamReader sr = new StreamReader(this.RadioDataPath))
            {
                // create output file
                string dir = Path.GetDirectoryName(this.OutputDataLocalPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using (StreamWriter sw = new StreamWriter(this.OutputDataLocalPath, false))
                {
                    // move the first line
                    string nextLine = sr.ReadLine();
                    sw.WriteLine(nextLine);

                    while (!sr.EndOfStream)
                    {
                        nextLine = sr.ReadLine();
                        sw.WriteLine("Processed-" + nextLine);
                    }
                } // using (StreamWriter sw = new StreamWriter(this.OutputDataPath, false))
            } // using (StreamReader sr = new StreamReader(this.RadioDataPath))

            Thread.Sleep(10 * 1000);

            Log.InfoFormat("task [{0}]/[{1}] is executed.", this.Name, this.ID);
        }

        public void Complete()
        {
            // put the data into task container
            if (this.ExecuteType == TaskExecuteType.Locally)
            {
                this.OwnContainer.SetTaskContext(this.Name, this.OutputDataLocalPath);
            }
            else
            {
                this.OwnContainer.SetTaskContext(this.Name, this.OutputDataFlyPath);
            }

            if (this.TaskCompleted != null)
            {
                this.TaskCompleted(this, null);
            }
        }

        public event ParallelTaskScheduler.Src.EventHandler TaskCompleted;

        public TaskContainer OwnContainer
        {
            get;
            set;
        }

        private string RadioDataPath
        {
            get;
            set;
        }

        private string OutputDataLocalPath
        {
            get
            {
                return string.Format(@"./temp/{0}/prepare.dat", this.ID);
            }
        }

        private string OutputDataFlyPath
        {
            get;
            set;
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
