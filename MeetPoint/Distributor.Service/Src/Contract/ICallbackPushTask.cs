using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Distributor.Service.Src.Contract
{
    public interface ICallbackPushTask
    {
        [OperationContract(IsOneWay=true)]
        void Display(string result);
    }
}
