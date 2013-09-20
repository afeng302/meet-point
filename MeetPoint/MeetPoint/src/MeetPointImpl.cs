using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MeetPoint.src
{
    class MeetPointImpl : IMeetPoint
    {
        ManualResetEvent mre = new ManualResetEvent(false);

        List<object> preCondArriveContextList = new List<object>();
        List<object> postArriveContextList = new List<object>();

        public MeetPointImpl(string ID, int preCondNumber, int postCondNumber)
            : this(ID, preCondNumber, postCondNumber, Timeout.Infinite)
        {
        }

        public MeetPointImpl(string ID, int preCondNumber, int postCondNumber, int millisecondsTimeout)
        {
            this.ID = ID;
            this.PreCondNumber = preCondNumber;
            this.PostCondNumber = postCondNumber;
            this.MillisecondsTimeout = millisecondsTimeout;

            this.PreCondArrivedCount = 0;
            this.PostCondArrivedCount = 0;
        }

        public string ID
        {
            get;
            private set;
        }

        public int PreCondNumber
        {
            get;
            private set;
        }

        public int PostCondNumber
        {
            get;
            private set;
        }

        public int PreCondArrivedCount
        {
            get;
            private set;
        }

        public int PostCondArrivedCount
        {
            get;
            private set;
        }

        public int MillisecondsTimeout
        {
            get;
            private set;
        }

        public bool IsBlocked
        {
            get
            {
                lock (this.mre)
                {
                    return !this.mre.WaitOne(0);
                }
            }
        }

        public void PreCondArrive(object arriveContext)
        {
            this.PreCondArrive(arriveContext, 1);
        }

        public void PreCondArrive(object arriveContext, int number)
        {
            lock(this.mre)
            {
                this.PreCondArrivedCount += number;

                if (this.PreCondArrivedCount >= this.PreCondNumber)
                {
                    this.mre.Set();
                }
            }

            if (arriveContext != null)
            {
                this.preCondArriveContextList.Add(arriveContext);
            }

            if (this.PreCondArrived != null)
            {
                this.PreCondArrived(this, new ArrivedEventArgs() { ArriveContext = arriveContext });
            }

            this.mre.WaitOne(this.MillisecondsTimeout);
        }

        public void PostCondArrive(object arriveContext)
        {
            this.PostCondArrive(arriveContext, 1);
        }

        public void PostCondArrive(object arriveContext, int number)
        {
            lock (this.mre)
            {
                this.PostCondArrivedCount += number;
            }

            if (arriveContext != null)
            {
                this.postArriveContextList.Add(arriveContext);
            }

            if (this.PostCondArrived != null)
            {
                this.PostCondArrived(this, new ArrivedEventArgs() { ArriveContext = arriveContext });
            }

            this.mre.WaitOne(this.MillisecondsTimeout);
        }

        public event EventHandler PreCondArrived;

        public event EventHandler PostCondArrived;
    }
}
