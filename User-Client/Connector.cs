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
        public void Run(string[] userInformation)
        {
            try
            {
                InitializationClient(userInformation);
                var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), this.port);
                using (tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        tcpSocket.Connect(tcpEndPoint);
                        Communication communication = new Communication(tcpSocket);
                        ChatMaster chatMaster = new ChatMaster(communication);
                        chatMaster.Run();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private void InitializationClient(string[] userInformation)
        {
            try
            {
                ip = userInformation[0];
                port = Convert.ToInt32(userInformation[1]);
                SaveData();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SaveData()
        {
            Console.WriteLine("If you want to save this date, click Enter");
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                fileMaster.AddPortAndIP(ip, port);
            }
        }
    }
}
