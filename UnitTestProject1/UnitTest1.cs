using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteDisk;
using System.IO;
using RemoteDisk.Interface;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            

            string UncPath = @"\\172.16.99.240\flume";
            //網域
            string Domain = "wneweb";
            
            //帳號
            string User = "user";
            //密碼
            string Passowrd = "password";

            IRemoteDisk remoteDisk = new DiskProxy(UncPath, Domain, User, Passowrd);
            remoteDisk.Rename(@"\\172.16.99.240\flume\pony.tmp", "pony.txt");

            
        }
    }
}
