﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MeetPoint.Src;
using System.Threading;

namespace MeetPoint.Test
{
    class PostCondTask
    {
        ArriveTask arriveTask = null;
        ManualResetEvent mre = new ManualResetEvent(false);

        public PostCondTask(IMeetPoint meetPoint, int arrivedCount)
        {
            this.arriveTask = new ArriveTask(meetPoint, arrivedCount, false);

            meetPoint.PostCondArrived += new Src.EventHandler(meetPoint_PostCondArrived);
        }

        void meetPoint_PostCondArrived(object sender, ArrivedEventArgs e)
        {
            if (e.ArriveContext == this.arriveTask)
            {
                this.mre.Set();
            }
        }

        public void RunAsync()
        {
            this.arriveTask.RunAsync();
        }

        public bool WaitArrive(int millisecondsTimeout)
        {
            // wait for arrived event
            return this.mre.WaitOne(millisecondsTimeout);
        }

        public bool WaitTaskUnblock(int millisecondsTimeout)
        {
            return this.arriveTask.WaitTaskUnblock(millisecondsTimeout);
        }

        public bool IsBlocked
        {
            get
            {
                return this.arriveTask.IsBlocked;
            }
        }
    }
}
