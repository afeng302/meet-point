using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;

namespace ParallelTaskScheduler.Src
{
    public delegate void EventHandler(Object sender, EventArgs e);

    public interface ITaskItem
    {
        string Name { get; set; }

        string ID { get; set; }

        bool IsDistributable { get; }

        TaskStatus Status { get; set; }

        TaskExecuteType ExecuteType { get; set; }

        ValueType[] BoxFlyParams();

        void UnboxFlyParams(ValueType[] flyParams);

        string[] BoxFlyFiles();

        void UnboxFlyFiles(string[] flyFiles);

        void Execute();

        void Complete();

        event EventHandler TaskCompleted;

        TaskContainer OwnContainer { get; set; }
    }
}
