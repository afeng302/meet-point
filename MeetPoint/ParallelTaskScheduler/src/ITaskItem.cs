using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParallelTaskScheduler.src
{
    public delegate void EventHandler(Object sender, EventArgs e);

    public interface ITaskItem
    {
        string Name
        {
            get;
            set;
        }

        void Execute();

        void Complete();

        event EventHandler TaskCompleted;
    }
}
