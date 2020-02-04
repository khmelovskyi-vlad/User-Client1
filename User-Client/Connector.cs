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
        private string nickname;
        private string password;
        private Socket tcpSocket;
        public Connector()
        {

        }
        public void Run(string[] userInformation)
        {
            try
            {
                InitializationClient(userInformation);
                var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                using (tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        tcpSocket.Connect(tcpEndPoint);
                        Communication communication = new Communication(tcpSocket);
                        ChatMaster chatMaster = new ChatMaster(communication);
                        chatMaster.Run();
                        //Entrance entrance = new Entrance(communication);
                        //entrance.Run();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        //private void Test(Communication communication, Socket socket)
        //{
        //    //communication.AnswerAndWriteServer();
        //    //Console.ReadKey();
        //    AnswerServer(tcpSocket, communication);
        //}
        //private byte[] buffer;
        //public StringBuilder data;
        //public void AnswerServer(Socket tcpSocket, Communication communication)
        //{
        //    var s = "IMG_20191203_201515.jpg";
        //    communication.AnswerAndWriteServer();
        //    var nameFile = communication.data.ToString();
        //    communication.SendMessage("ok");
        //    using (var stream = File.Open($@"D:\temp\ok\{nameFile}", FileMode.OpenOrCreate, FileAccess.Write))
        //    {
        //        buffer = new byte[256];
        //        data = new StringBuilder();
        //        do
        //        {
        //            var size = tcpSocket.Receive(buffer);
        //            stream.Write(buffer, 0, size);
        //            //data.Append(Encoding.ASCII.GetString(buffer, 0, size));
        //        } while (tcpSocket.Available > 0);
        //    }
        //}
        private void InitializationClient(string[] userInformation)
        {
            ip = userInformation[0];
            port = Convert.ToInt32(userInformation[1]);
            nickname = userInformation[2];
            password = userInformation[3];
        }
    }
}
