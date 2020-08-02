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
        public async Task AnswerAndWriteToSecndWindow(SecondWindowServer secondWindowServer)
        {
            await AnswerServer();
            secondWindowServer.Write(data.ToString());
        }
        public async Task AnswerAndWriteServer()
        {
            await AnswerServer();
            Console.WriteLine(data);
        }
        public async Task AnswerServer()
        {
            buffer = new byte[size];
            data = new StringBuilder();
            do
            {
                try
                {
                    var received = await Task.Factory.FromAsync(tcpSocket.BeginReceive(buffer, 0, size, SocketFlags.None, null, null),
                        tcpSocket.EndReceive);
                    data.Append(Encoding.ASCII.GetString(buffer, 0, received));
                }
                catch (Exception)
                {
                    Console.WriteLine("Server forcefully disconnected");
                    data = new StringBuilder();
                    data.Append("?/you left the chat");
                    return;
                }
            } while (tcpSocket.Available > 0);
        }
        public async Task SendMessage(string message)
        {
            if (message == "??")
            {
                throw new OperationCanceledException();
            }
            byte[] byteData = Encoding.ASCII.GetBytes(message);
            await Task.Factory.FromAsync(tcpSocket.BeginSend(byteData, 0, byteData.Length, 0, null, null), tcpSocket.EndReceive);
        }
        public async Task SendFile(string path, string fileName)
        {
            await SendMessage(fileName);
            await AnswerServer();
            var length = new FileInfo(path).Length;
            var lengthByte = BitConverter.GetBytes(length);
            await Task.Factory.FromAsync(
                tcpSocket.BeginSendFile(path, lengthByte, null, TransmitFileOptions.UseDefaultWorkerThread, null, null),
                tcpSocket.EndSendFile);
        }
        public async Task ReciveFile(string path)
        {
            var byteCount = 8;
            var bufferFileLength = new byte[byteCount];
            await Task.Factory.FromAsync(tcpSocket.BeginReceive(bufferFileLength, 0, byteCount, SocketFlags.None, null, null),
                tcpSocket.EndReceive);
            var fileLength = BitConverter.ToInt64(bufferFileLength, 0);
            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                buffer = new byte[size];
                do
                {
                    var received = await Task.Factory.FromAsync(tcpSocket.BeginReceive(buffer, 0, size, SocketFlags.None, null, null),
                        tcpSocket.EndReceive);
                    await stream.WriteAsync(buffer, 0, received);
                } while (stream.Length != fileLength);
            }
        }
    }
}
