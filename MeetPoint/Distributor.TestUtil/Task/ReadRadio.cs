using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using ParallelTaskScheduler.Src;
using log4net;
using System.Reflection;
using System.IO;
using System.Threading;

namespace Distributor.TestUtil.Task
{
    public class ReadRadio : ITaskItem
    {
        public string Name
        {
            get
            {
                return "Read-Radio";
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
            get { return false; }
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

            // create temp file
            string dir = Path.GetDirectoryName(this.RadioDataPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (StreamWriter sw = new StreamWriter(this.RadioDataPath, false))
            {
                // the first line is GUID of this task
                sw.WriteLine(this.ID);

                // place holders
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        sw.Write(j.ToString() + "-");
                    }
                    sw.WriteLine();
                }
            }

            Thread.Sleep(10 * 1000);

            Log.InfoFormat("task [{0}]/[{1}] is executed", this.Name, this.ID);
        }

        public void Complete()
        {
            // put the data into task container
            this.OwnContainer.SetTaskContext(this.Name, this.RadioDataPath);

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
                return string.Format(@"./temp/{0}/read-radio.dat", this.ID);
            }
        }

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
