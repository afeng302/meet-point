using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Distributor.Service.Src.Contract
{
    [Serializable]
    public class TransferTaskItem
    {
        public string Name { get; set; }

        public string ID { get; set; }

        public string TypeName { get; set; }

        public ValueType[] Params { get; set; }
    }
}
