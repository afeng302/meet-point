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

        bool IsDistributable { get; }

        void Execute();

        void Complete();

        event EventHandler TaskCompleted;
    }
}
