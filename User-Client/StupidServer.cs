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
    class StupidServer
    {
        public StupidServer()
        {
            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var tcpEndPoint = new IPEndPoint(IPAddress.Any, port);
            tcpSocket.Bind(tcpEndPoint);
            tcpSocket.Listen(5);
        }
        private const int port = 1235;
        private Socket tcpSocket;
        private Socket listener;
        AutoResetEvent close = new AutoResetEvent(false);
        public void Run()
        {
            Task.Run(() => tcpSocket.BeginAccept(ar =>
            {
                try
                {
                    listener = tcpSocket.EndAccept(ar);
                    close.WaitOne();

                }
                catch (Exception socketException)
                {
                    throw socketException;
                }
            }, tcpSocket));
        }
        public void AnswerServer(string message)
        {
            while (true)
            {
                try
                {
                    SendMessage(message);
                    if (message == "?/end")
                    {
                        close.Set();
                    }
                    return;
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("lol");
                    continue;
                }
            }
        }
        public void SendMessage(string message)
        {
            listener.Send(Encoding.ASCII.GetBytes(message));
        }
    }
}
