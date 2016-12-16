using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDisk.Interface
{
    public interface IRemoteDisk
    {
        bool Connect();

        bool Disconnect();

        void UploadFile(string localPath, string remotePath);

        void DownLoadFile(string remotePath, string localPath);

        bool FileExists(string remotePath);

        bool DirectoryExists(string remotePath);

        void Rename(string oldNamePath, string newName);

        void DeleteFile(string remotePath);

        void DeleteFolder(string reomtePath);

        void CreateDirectory(string remotePath);


    }
}
