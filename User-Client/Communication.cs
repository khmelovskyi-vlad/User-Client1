using System;
using System.Collections.Generic;
using System.IO;
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
        private const int size = 256;
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
        public void SendFile(string path, string fileName)
        {
            SendMessage(fileName);
            AnswerServer();
            tcpSocket.SendFile(path);
        }
        public void ReciveFile(string path)
        {
            buffer = new byte[size];
            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                do
                {
                    var size = tcpSocket.Receive(buffer);
                    stream.Write(buffer, 0, size);
                } while (tcpSocket.Available > 0);
            }
        }
    }
}
