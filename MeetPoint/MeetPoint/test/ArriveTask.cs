using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MeetPoint.src;
using System.Threading;

namespace MeetPoint.test
{
    class ArriveTask
    {
        BackgroundWorker bgWorker = new BackgroundWorker();

        ManualResetEvent mre = new ManualResetEvent(false);

        bool isPreCond = false;
        IMeetPoint meetPoint = null;
        int arrivedCount = 0;

        public ArriveTask(IMeetPoint meetPoint, int arrivedCount, bool isPreCond)
        {
            this.meetPoint = meetPoint;
            this.arrivedCount = arrivedCount;
            this.isPreCond = isPreCond;
            this.IsBlocked = true;

            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.isPreCond)
            {
                this.meetPoint.PreCondArrive(this, this.arrivedCount);
            }
            else
            {
                this.meetPoint.PostCondArrive(this, this.arrivedCount);
            }

            this.IsBlocked = false;
            this.mre.Set();
        }

        public void RunAsync()
        {
            this.bgWorker.RunWorkerAsync();
        }

        public bool WaitTaskUnblock(int millisecondsTimeout)
        {
            return this.mre.WaitOne(millisecondsTimeout);
        }

        public bool IsBlocked
        {
            get;
            private set;
        }
    }
}
