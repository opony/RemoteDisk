using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteDisk;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //string UncPath = @"\\172.17.101.135\edsa";
            ////網域
            //string Domain = "wneweb";
            ////帳號
            //string User = "10509014";
            ////密碼
            //string Passowrd = "0110pony";

            string UncPath = @"\\172.16.99.240\flume";
            //網域
            string Domain = "wneweb";
            
            //帳號
            string User = "user";
            //密碼
            string Passowrd = "password";

            DiskProxy unc = new DiskProxy();
            unc.Connect(UncPath, Domain, User, Passowrd);

            Assert.IsTrue(unc.IsConnect, "test connect fail.");
            if (unc.IsConnect)
            {
                Console.WriteLine("已連線到 " + UncPath);
                //Console.ReadKey();
                
                using (StreamWriter writer = new StreamWriter(@"\\172.16.99.240\flume\pony.tmp"))
                {
                    writer.WriteLine("test");
                }

                File.Move(@"\\172.16.99.240\flume\pony.tmp", @"\\172.16.99.240\flume\pony.txt");

                string[] dirs = Directory.GetFiles(@"\\172.16.99.240\flume");

                Assert.AreNotEqual<int>(dirs.Length, 0, "get folders fail.");

                unc.Disconnect();
                Console.WriteLine("已中斷到 " + UncPath);
            }
            else
            {
                Console.WriteLine(UncPath + " 連線失敗");
                Console.WriteLine("錯誤代碼 " + unc.LastError.ToString());
                Console.ReadKey();
            }
        }
    }
}
