using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributor.Service.Src.Contract;
using System.ServiceModel;
using System.IO;
using System.Reflection;

namespace Distributor.Service.Src.Service
{
    [ServiceBehavior]
    public class FileRepositoryService : IFileRepositoryService
    {
        public FileRepositoryService()
        {
            this.RepositoryDirectory
                = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "file_repo_service");
        }

        /// <summary>
        /// Gets or sets the repository directory.
        /// </summary>
        public string RepositoryDirectory { get; set; }

        /// <summary>
        /// Gets a file from the repository
        /// </summary>
        public System.IO.Stream GetFile(string virtualPath)
        {
            string filePath = Path.Combine(RepositoryDirectory, virtualPath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File was not found", Path.GetFileName(filePath));
            }

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Uploads a file into the repository
        /// </summary>
        public void PutFile(FileUploadMessage msg)
        {
            string filePath = Path.Combine(RepositoryDirectory, msg.VirtualPath);
            string dir = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var outputStream = new FileStream(filePath, FileMode.Create))
            {
                msg.DataStream.CopyTo(outputStream);
            }
        }

        /// <summary>
        /// Deletes a file from the repository
        /// </summary>
        public void DeleteFile(string virtualPath)
        {
            string filePath = Path.Combine(RepositoryDirectory, virtualPath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Lists files from the repository at the specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path. This can be null to list files from the root of
        /// the repository.</param>
        public StorageFileInfo[] GetFileInfo(string virtualPath)
        {
            string basePath = RepositoryDirectory;

            if (!string.IsNullOrEmpty(virtualPath))
            {
                basePath = Path.Combine(RepositoryDirectory, virtualPath);
            }

            // if the virtual path point to a specific file, return it directly.
            if (File.Exists(basePath))
            {
                FileInfo info = new FileInfo(basePath);
                return new StorageFileInfo[1] 
                    {
                        new StorageFileInfo{ Size = info.Length, VirtualPath = virtualPath}
                    };
            }

            // if the folder does not exist, return null
            if (!Directory.Exists(basePath))
            {
                return null;
            }

            // return the files in the specific folder
            DirectoryInfo dirInfo = new DirectoryInfo(basePath);
            FileInfo[] files = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);

            return (from f in files
                    select new StorageFileInfo()
                    {
                        Size = f.Length,
                        VirtualPath = f.FullName.Substring(f.FullName.IndexOf(RepositoryDirectory) + RepositoryDirectory.Length + 1)
                    }).ToArray();
        }
    }
}
