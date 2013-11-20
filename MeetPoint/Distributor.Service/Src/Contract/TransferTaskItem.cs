using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Distributor.Service.Src.Contract
{
    public enum TransferType
    {
        Unknown,
        Request,
        Response
    }

    public enum TaskStatus
    {
        Unknown,
        Ready,
        Running,
        Completed,
        Aborted,
        Timeout
    }

    [Serializable]
    public class TransferTaskItem
    {
        public string Name { get; set; }

        public string ID { get; set; }

        public string TypeName { get; set; }

        public string SrcNode { get; set; }

        public string DestNode { get; set; }

        public TransferType TaskTransferType { get; set; }

        public ValueType[] Params { get; set; }

        public string[] Files { get; set; }

        public TaskStatus Status { get; set; }

        public string Message { get; set; }
    }
}
