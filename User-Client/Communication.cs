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
            try
            {
                var mesLength = await FindMessageLength();
                buffer = new byte[size];
                data = new StringBuilder();
                do
                {
                    var received = await Task.Factory.FromAsync(tcpSocket.BeginReceive(buffer, 0, size, SocketFlags.None, null, null), tcpSocket.EndReceive);
                    data.Append(Encoding.ASCII.GetString(buffer, 0, received));
                } while (mesLength != data.Length);
            }
            catch (Exception)
            {
                Console.WriteLine("Server forcefully disconnected");
                data = new StringBuilder();
                data.Append("?/you left the chat");
                return;
            }
        }
        public async Task SendMessage(string message)
        {
            if (message == "??")
            {
                throw new OperationCanceledException();
            }
            byte[] byteData = Encoding.ASCII.GetBytes(message);
            var mesLengthByte = CreateFirstMessage(byteData.Length);
            await Task.Factory.FromAsync(tcpSocket.BeginSend(mesLengthByte, 0, mesLengthByte.Length, SocketFlags.None, null, null), tcpSocket.EndSend);
            await Task.Factory.FromAsync(tcpSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, null, null), tcpSocket.EndSend);
        }






        //private byte[] CreateFirstMessage(long byteLength)
        //{
        //    var arrayByteLanght = BitConverter.GetBytes(byteLength);
        //    if (byteLength < 65535)
        //    {
        //        return new byte[3] { arrayByteLanght[0], arrayByteLanght[1], 1 };
        //    }
        //    else if (byteLength < 4294967295)
        //    {
        //        return new byte[6] { arrayByteLanght[0], arrayByteLanght[1], 0, arrayByteLanght[2], arrayByteLanght[3], 1 };
        //    }
        //    else
        //    {
        //        return new byte[10] { arrayByteLanght[0], arrayByteLanght[1], 0, arrayByteLanght[2], arrayByteLanght[3], 0,
        //        arrayByteLanght[4], arrayByteLanght[5], arrayByteLanght[6], arrayByteLanght[7] };
        //    }
        //}
        //private async Task<long> FindMessageLength()
        //{
        //    var byteCount = 3;
        //    var resultBuffer = new List<byte>();
        //    var buffer = new byte[byteCount];
        //    var needAdd = true;
        //    for (int i = 0; i < 2; i++)
        //    {
        //        await Task.Factory.FromAsync(tcpSocket.BeginReceive(buffer, 0, byteCount, SocketFlags.None, null, null), tcpSocket.EndReceive);
        //        resultBuffer.Add(buffer[0]);
        //        resultBuffer.Add(buffer[1]);
        //        if (buffer[2] == 1)
        //        {
        //            needAdd = false;
        //            break;
        //        }
        //        buffer = new byte[byteCount];
        //    }
        //    if (needAdd)
        //    {
        //        buffer = new byte[4];
        //        await Task.Factory.FromAsync(tcpSocket.BeginReceive(buffer, 0, byteCount, SocketFlags.None, null, null), tcpSocket.EndReceive);
        //        resultBuffer.AddRange(buffer);
        //    }
        //    if (resultBuffer.Count() == 2)
        //    {
        //        return BitConverter.ToUInt16(resultBuffer.ToArray(), 0);
        //    }
        //    else if (resultBuffer.Count() == 4)
        //    {
        //        return BitConverter.ToUInt32(resultBuffer.ToArray(), 0);
        //    }
        //    else
        //    {
        //        return BitConverter.ToInt64(resultBuffer.ToArray(), 0);
        //    }
        //}
        private byte[] CreateFirstMessage(long byteLength)
        {
            var arrayByteLanght = BitConverter.GetBytes(byteLength);
            if (byteLength < 65535)
            {
                return new byte[3] { 2, arrayByteLanght[0], arrayByteLanght[1] };
            }
            else if (byteLength < 4294967295)
            {
                return new byte[5] { 4, arrayByteLanght[0], arrayByteLanght[1], arrayByteLanght[2], arrayByteLanght[3] };
            }
            else
            {
                return new byte[9] { 8, arrayByteLanght[0], arrayByteLanght[1], arrayByteLanght[2], arrayByteLanght[3],
                arrayByteLanght[4], arrayByteLanght[5], arrayByteLanght[6], arrayByteLanght[7] };
            }
        }
        private async Task<long> FindMessageLength()
        {
            var resultBuffer = new List<byte>();
            var byteCount = 1;
            var buffer = new byte[byteCount];
            await Task.Factory.FromAsync(tcpSocket.BeginReceive(buffer, 0, byteCount, SocketFlags.None, null, null), tcpSocket.EndReceive);
            return await GetBufferLength(buffer[0]);
        }
        private async Task<long> GetBufferLength(int byteCount)
        {
            var buffer = new byte[byteCount];
            await Task.Factory.FromAsync(tcpSocket.BeginReceive(buffer, 0, byteCount, SocketFlags.None, null, null), tcpSocket.EndReceive);
            if (byteCount == 2)
            {
                return BitConverter.ToUInt16(buffer, 0);
            }
            else if (byteCount == 4)
            {
                return BitConverter.ToUInt32(buffer, 0);
            }
            else
            {
                return BitConverter.ToInt64(buffer, 0);
            }
        }





        public async Task SendFile(string path, string fileName)
        {
            await SendMessage(fileName);
            await AnswerServer();
            var lengthByte = CreateFirstMessage(new FileInfo(path).Length);
            await Task.Factory.FromAsync(
                tcpSocket.BeginSendFile(path, lengthByte, null, TransmitFileOptions.UseDefaultWorkerThread, null, null),
                tcpSocket.EndSendFile);
        }
        public async Task ReciveFile(string path)
        {
            var fileLength = await FindMessageLength();
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
