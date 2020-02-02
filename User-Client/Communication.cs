using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class Communication
    {
        public Communication(Socket tcpSocket)
        {
            this.tcpSocket = tcpSocket;
        }
        private Socket tcpSocket { get; }
        private byte[] buffer;
        public StringBuilder data;
        public void AnswerAndWriteServer()
        {
            AnswerServer();
            Console.WriteLine(data);
        }
        public void AnswerServer()
        {
            buffer = new byte[256];
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
