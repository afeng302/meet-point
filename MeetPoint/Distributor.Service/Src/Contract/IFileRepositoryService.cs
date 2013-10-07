using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;

namespace Distributor.Service.Src.Contract
{
    [ServiceContract]
    public interface IFileRepositoryService
    {
        [OperationContract]
        Stream GetFile(string virtualPath);

        [OperationContract]
        void PutFile(FileUploadMessage msg);

        [OperationContract]
        void DeleteFile(string virtualPath);

        [OperationContract]
        StorageFileInfo[] GetFileInfo(string virtualPath);
    }
}
