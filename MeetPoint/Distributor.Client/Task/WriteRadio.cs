using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParallelTaskScheduler.Src;
using log4net;
using System.Reflection;
using System.Threading;
using System.IO;

namespace Distributor.Client.Task
{
    class WriteRadio : ITaskItem
    {
        public string Name
        {
            get
            {
                return "Write-Radio";
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
                throw new NotImplementedException();
            }
        }

        public bool IsDistributable
        {
            get { return false; }
        }

        public Service.Src.Contract.TaskStatus Status
        {
            get;
            set;
        }

        public Service.Src.Contract.TaskExecuteType ExecuteType
        {
            get;
            set;
        }

        public ValueType[] BoxFlyParams()
        {
            throw new NotImplementedException();
        }

        public void UnboxFlyParams(ValueType[] flyParams)
        {
            throw new NotImplementedException();
        }

        public string[] BoxFlyFiles()
        {
            throw new NotImplementedException();
        }

        public void UnboxFlyFiles(string[] flyFiles)
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            Log.InfoFormat("task [{0}]/[{1}] is executing ...", this.Name, this.ID);

            // prepare the radio data
            string inputFilePath = (string)this.OwnContainer.GetTaskContext("Prepare-Data");
            using (StreamReader sr = new StreamReader(inputFilePath))
            {
                using (StreamWriter sw = new StreamWriter(this.RadioDataPath, false))
                {
                    // copy the first line
                    string nextLine = sr.ReadLine();
                    sw.WriteLine(nextLine);

                    while (!sr.EndOfStream)
                    {
                        nextLine = sr.ReadLine();
                        sw.WriteLine("Written-" + nextLine);
                    }
                }
            }

            Thread.Sleep(10 * 1000);

            Log.InfoFormat("task [{0}]/[{1}] is executed", this.Name, this.ID);
        }

        public void Complete()
        {
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
            get
            {
                return string.Format(@"./temp/{0}/write-radio.dat", this.ID);
            }
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
