using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class Connector
    {
        private string ip;
        private int port;
        private Socket tcpSocket;
        public Connector(FileMaster fileMaster)
        {
            this.fileMaster = fileMaster;
        }
        private FileMaster fileMaster;
        public async Task Run()
        {
            try
            {
                var userInformation = await FindNeedData(fileMaster);
                await InitializationClient(userInformation);
                var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), this.port);
                using (tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        await Task.Factory.FromAsync(tcpSocket.BeginConnect(tcpEndPoint, null, null), tcpSocket.EndConnect);
                        Communication communication = new Communication(tcpSocket);
                        ChatMaster chatMaster = new ChatMaster(communication);
                        await chatMaster.Run();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
            }
        }
        private async Task InitializationClient(string[] userInformation)
        {
            try
            {
                ip = userInformation[0];
                port = Convert.ToInt32(userInformation[1]);
                await SaveData();
            }
            catch (FormatException ex)
            {
                Console.WriteLine("The port isn't the number");
                throw ex;
            }
        }
        private async Task SaveData()
        {
            Console.WriteLine("If you want to save this date, click Enter");
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                await fileMaster.AddPortAndIP(ip, port);
            }
        }

        private async Task<string[]> FindNeedData(FileMaster fileMaster)
        {
            await ReadShowData<string>(fileMaster, @"D:\temp\User\IPs.json", "IPs:");
            await ReadShowData<int>(fileMaster, @"D:\temp\User\ports.json", "Ports:");
            return EnterData();
        }
        private async Task ReadShowData<T>(FileMaster fileMaster, string path, string firstMessage)
        {
            Console.WriteLine(firstMessage);
            var data = await fileMaster.ReadData<T>(path);
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
            var ip = "192.168.1.11";
            var port = "1234";
            //var ip = EnterSomeData("Enter your IP");
            //var port = EnterSomeData("Enter need port");
            return new string[] { ip, port };
        }
        private string EnterSomeData(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }
    }
}
