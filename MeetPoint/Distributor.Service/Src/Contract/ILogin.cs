using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Distributor.Service.Src.Contract
{
    [ServiceContract (CallbackContract=typeof(ICallbackPushTask))]
    public interface ILogin
    {
        [OperationContract]
        void Login(string clientHostName);
    }
}
