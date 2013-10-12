using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelTaskScheduler.Src
{
    public delegate void EventHandler(Object sender, EventArgs e);

    public interface ITaskItem
    {
        string Name { get; set; }

        string ID { get; }

        bool IsDistributable { get; }

        ValueType[] BoxFlyParams();

        void UnboxFlyParams(ValueType[] flyParams);

        void Execute();

        void Complete();

        event EventHandler TaskCompleted;
    }
}
