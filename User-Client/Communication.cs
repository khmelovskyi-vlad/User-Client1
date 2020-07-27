using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

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
        public void AnswerAndWriteToSecndWindow(SecondWindowServer secondWindowServer)
        {
            AnswerServer();
            secondWindowServer.Write(data.ToString());
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
        public void SendFile(string path, string fileName)
        {
            SendMessage(fileName);
            AnswerServer();
            var length = new FileInfo(path).Length;
            var lengthByte = BitConverter.GetBytes(length);
            //tcpSocket.SendFile(path);
            tcpSocket.SendFile(path, lengthByte, null, TransmitFileOptions.UseKernelApc);
            //tcpSocket.BeginSendFile(path, lengthByte, null, TransmitFileOptions.UseKernelApc, SendFileCallback, tcpSocket);
            //resetSend.WaitOne();
        }
        public void ReciveFile(string path)
        {
            buffer = new byte[8];
            var bufferSize = tcpSocket.Receive(buffer);
            var fileLength = BitConverter.ToInt64(buffer, 0);
            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                buffer = new byte[size];
                var endSend = 0;
                do
                {
                    bufferSize = tcpSocket.Receive(buffer);
                    endSend = endSend + bufferSize;
                    stream.Write(buffer, 0, bufferSize);
                } while (endSend != fileLength);
            }
        }
        //public void SendFile(string fileName)
        //{
        //    var length = new FileInfo(fileName).Length;
        //    var lengthByte = BitConverter.GetBytes(length);
        //    tcpSocket.BeginSendFile(fileName, lengthByte, null, TransmitFileOptions.UseKernelApc, SendFileCallback, tcpSocket);
        //    resetSend.WaitOne();
        //}
        //AutoResetEvent resetSend = new AutoResetEvent(false);
        //private void SendFileCallback(IAsyncResult AR)
        //{
        //    Socket current = (Socket)AR.AsyncState;
        //    try
        //    {
        //        current.EndSendFile(AR);
        //    }
        //    catch (SocketException)
        //    {
        //        Console.WriteLine("Can`t send file");
        //    }
        //    resetSend.Set();
        //}
    }
}
