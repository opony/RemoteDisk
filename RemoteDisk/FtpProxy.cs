using RemoteDisk.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDisk
{
    public enum FtpItemType { Folder, File };
    public class FtpItem
    {
        private string detail;
        public FtpItem(string detail)
        {
            this.detail = detail;

            string[] tokens = detail.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            this.Name = tokens[8];
            if (tokens[0].StartsWith("d"))
                this.Type = FtpItemType.Folder;
            else
                this.Type = FtpItemType.File;


        }

        public string Name
        { get; set; }

        public FtpItemType Type
        { get; set; }

        
    }
    public class FtpProxy : IRemoteDisk
    {
        string userName;
        string pwd;

        public FtpProxy(string userName, string pwd)
        {
            this.userName = userName;
            this.pwd = pwd;
        }

        private FtpWebRequest GetFtpRequest(string uri, string ftpMethod)
        {
            //FtpWebRequest requestFileDownload = (FtpWebRequest)WebRequest.Create(System.IO.Path.Combine(this.rootUri, uri));
            FtpWebRequest requestFileDownload = (FtpWebRequest)WebRequest.Create(uri);
            requestFileDownload.Credentials = new NetworkCredential(this.userName, this.pwd);
            requestFileDownload.Method = ftpMethod;

            return requestFileDownload;
        }



        public bool Connect()
        {
            //FTP 不須做 connect
            return true;
        }

        public void CreateDirectory(string remotePath)
        {
            FtpWebRequest request = GetFtpRequest(remotePath, WebRequestMethods.Ftp.MakeDirectory);
            using (var resp = (FtpWebResponse)request.GetResponse())
            {
                //Console.WriteLine(resp.StatusCode);
            }
        }

        public void DeleteFile(string remotePath)
        {
            FtpWebRequest request = GetFtpRequest(remotePath, WebRequestMethods.Ftp.DeleteFile);
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {

            }
        }

        public void DeleteFolder(string reomtePath)
        {
            List<FtpItem> ftpItemList = DirectoryListing(reomtePath);
            string path;
            foreach (FtpItem ftpItem in ftpItemList)
            {
                if (ftpItem.Name == "." || ftpItem.Name == "..")
                    continue;

                path = reomtePath + "/" + ftpItem.Name;
                if (ftpItem.Type == FtpItemType.Folder)
                    DeleteFolder(path);
                else
                    DeleteFile(path);

            }

            FtpWebRequest request = GetFtpRequest(reomtePath, WebRequestMethods.Ftp.RemoveDirectory);
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {

            }
        }

        public List<FtpItem> DirectoryListing(string folderPath)
        {
            List<FtpItem> result = new List<FtpItem>();
            FtpWebRequest request = GetFtpRequest(folderPath, WebRequestMethods.Ftp.ListDirectoryDetails);
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);


                while (!reader.EndOfStream)
                {
                    result.Add(new FtpItem(reader.ReadLine()));
                }

                reader.Close();
            }

            return result;
        }

        public bool DirectoryExists(string remotePath)
        {
            FtpWebRequest requestFileDownload = GetFtpRequest(remotePath, WebRequestMethods.Ftp.GetDateTimestamp);

            FtpWebResponse response = null;
            StreamReader reader = null;
            try
            {
                response = (FtpWebResponse)requestFileDownload.GetResponse();

                Stream responseStream = response.GetResponseStream();
                reader = new StreamReader(responseStream);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine("");
                }
            }
            catch (Exception)
            {

                return false;

            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (response != null)
                    response.Close();
            }

            return true;
        }

        public bool Disconnect()
        {
            //Ftp 不須 disconnect
            return true;
        }

        public void DownLoadFile(string remotePath, string localPath)
        {
            FtpWebRequest request = GetFtpRequest(remotePath, WebRequestMethods.Ftp.DownloadFile);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();

            FileStream fileStream = new FileStream(localPath, FileMode.Create);
            int bytesRead = 0;
            byte[] buffer = new byte[2048];
            while (true)
            {
                bytesRead = responseStream.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                    break;

                fileStream.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
        }

        public bool FileExists(string remotePath)
        {
            FtpWebRequest request = GetFtpRequest(remotePath, WebRequestMethods.Ftp.GetFileSize);
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)request.GetResponse();
                long size = response.ContentLength;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            return true;
        }

        public void Rename(string oldNamePath, string newName)
        {
            FtpWebRequest request = GetFtpRequest(oldNamePath, WebRequestMethods.Ftp.Rename);
            request.RenameTo = newName;
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
            }
        }

        public void UploadFile(string localPath, string remotePath)
        {
            FtpWebRequest request = GetFtpRequest(remotePath, WebRequestMethods.Ftp.UploadFile);
            request.UseBinary = true;
            byte[] dataBytes = File.ReadAllBytes(localPath);
            request.ContentLength = dataBytes.Length;
            using (Stream s = request.GetRequestStream())
            {
                s.Write(dataBytes, 0, dataBytes.Length);
            }

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            //Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

            response.Close();
        }
    }
}
