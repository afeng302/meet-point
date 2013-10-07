using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Distributor.Service.Src.Contract
{
    [ServiceContract]
    public interface ICapacityFactor
    {
        [OperationContract]
        void ReportFactor(int capacityFactor);
    }
}
