using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeetPoint.Src
{
    public class ArrivedEventArgs : EventArgs
    {
        public object ArriveContext
        {
            get;
            set;
        }
    }
}
