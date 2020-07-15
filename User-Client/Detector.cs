using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class Detector
    {
        public Detector()
        {
        }
        public void Run()
        {
            FileMaster fileMaster = new FileMaster();
            var connector = new Connector(fileMaster);
            try
            {
                while (true)
                {
                    var userInformation = FindNeedData(fileMaster);
                    connector.Run(userInformation);
                    Console.WriteLine("If you want connect again, click Enter");
                    var key = Console.ReadKey(true);
                    if (key.Key != ConsoleKey.Enter)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }

        }
        private string[] FindNeedData(FileMaster fileMaster)
        {
            ReadShowData(fileMaster, @"D:\temp\User\IPs.json", "IPs:");
            ReadShowData(fileMaster, @"D:\temp\User\ports.json", "Ports:");
            //var IPs = (List<string>)fileMaster.ReadData(@"D:\temp\User\IPs.json");
            //if (IPs != null)
            //{
            //    foreach (var IP in IPs)
            //    {
            //        Console.WriteLine(IP);
            //    }
            //}
            //var ports = (List<int>)fileMaster.ReadData(@"D:\temp\User\ports.json");
            //if (ports != null)
            //{
            //    foreach (var port in ports)
            //    {
            //        Console.WriteLine(port);
            //    }
            //}
            return EnterData();
        }
        private void ReadShowData(FileMaster fileMaster, string path, string firstMessage)
        {
            Console.WriteLine(firstMessage);
            var data = (List<object>)fileMaster.ReadData(path);
            if (data != null)
            {
                foreach (var oneData in data)
                {
                    Console.WriteLine(oneData);
                }
            }
        }
        private string[] EnterData()
        {
            var ip = "192.168.1.10";
            var port = "1234";
            //var ip = EnterSomeData("Enter your IP");
            //var port = EnterSomeData("Enter need port");
            return new string[] { ip, port};
        }
        private string EnterSomeData(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }
    }
}
