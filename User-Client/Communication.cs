using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        AutoResetEvent resetSend = new AutoResetEvent(true);
        AutoResetEvent resetReceive = new AutoResetEvent(false);
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
                tcpSocket.BeginReceive(buffer, 0, size, SocketFlags.None, ReceiveCallback, tcpSocket);
                resetReceive.WaitOne();
            } while (tcpSocket.Available > 0);
        }
        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Server forcefully disconnected");
                resetReceive.Set();
                return;
            }
            data.Append(Encoding.ASCII.GetString(buffer, 0, received));
            resetReceive.Set();
        }
        public void SendMessage(string message)
        {
            if (message == "??")
            {
                throw new OperationCanceledException();
            }
            resetSend.WaitOne();
            byte[] byteData = Encoding.ASCII.GetBytes(message);
            tcpSocket.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, tcpSocket);
        }
        private void SendCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            try
            {
                current.EndSend(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Can`t send message");
            }
            resetSend.Set();
        }
        public void SendFile(string path, string fileName)
        {
            SendMessage(fileName);
            AnswerServer();
            var length = new FileInfo(path).Length;
            var lengthByte = BitConverter.GetBytes(length);
            //tcpSocket.ReceiveAsync(path, lengthByte, null, TransmitFileOptions.UseKernelApc);
            tcpSocket.BeginSendFile(path, lengthByte, null, TransmitFileOptions.UseKernelApc, SendFileCallback, tcpSocket);
            resetSend.WaitOne();
        }
        public void ReciveFile(string path)
        {
            buffer = new byte[8];
            var bufferSize = tcpSocket.Receive(buffer);
            var fileLength = BitConverter.ToInt64(buffer, 0);
            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                buffer = new byte[size];
                do
                {
                    bufferSize = tcpSocket.Receive(buffer);
                    stream.Write(buffer, 0, bufferSize);
                } while (stream.Length != fileLength);
            }
        }
        private void SendFileCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            try
            {
                current.EndSendFile(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Can`t send file");
            }
            resetSend.Set();
        }
    }
}
