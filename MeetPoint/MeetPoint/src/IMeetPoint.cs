using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeetPoint.Src
{
    public delegate void EventHandler(Object sender, ArrivedEventArgs e);

    public interface IMeetPoint
    {
        string ID
        {
            get;
        }

        int PreCondNumber
        {
            get;
        }

        int PostCondNumber
        {
            get;
        }

        int PreCondArrivedCount
        {
            get;
        }

        int PostCondArrivedCount
        {
            get;
        }

        int MillisecondsTimeout
        {
            get;
        }

        bool IsBlocked
        {
            get;
        }

        void PreCondArrive(object arriveContext);

        void PreCondArrive(object arriveContext, int number);

        void PostCondArrive(object arriveContext);

        void PostCondArrive(object arriveContext, int number);

        void Reset();

        event EventHandler PreCondArrived;

        event EventHandler PostCondArrived;
    }
}
