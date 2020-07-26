using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace User_Client
{
    class StupidConnector
    {
        private string ip = "192.168.1.11";
        private int port = 1235;
        private Socket tcpSocket;
        public void Run()
        {
            try
            {
                var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                using (tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        tcpSocket.Connect(tcpEndPoint);
                        AnswerAndWrite();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("first");
                        Console.WriteLine(ex);
                        Console.Read();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("second");
                Console.WriteLine(ex);
                Console.Read();
            }
        }
        private byte[] buffer;
        public StringBuilder data;
        private const int size = 256;
        private void AnswerAndWrite()
        {
            while (true)
            {
                AnswerAndWriteServer();
                if (data.ToString() == "?/end")
                {
                    return;
                }
            }
        }
        public void AnswerAndWriteServer()
        {
            AnswerServer();
            Console.WriteLine(data);
        }
        public void AnswerServer()
        {
            buffer = new byte[size];
            data = new StringBuilder();
            do
            {
                var size = tcpSocket.Receive(buffer);
                data.Append(Encoding.ASCII.GetString(buffer, 0, size));
            } while (tcpSocket.Available > 0);
        }
        public void SendMessage(string message)
        {
            tcpSocket.Send(Encoding.ASCII.GetBytes(message));
        }
    }
}
