using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class FileMaster
    {
        private const string PathIps = @"D:\temp\User\IPs.json";
        private const string PathPorts = @"D:\temp\User\ports.json";
        public async Task AddPortAndIP<T, T2>(T IP, T2 port)
        {
            await AddData(IP, PathIps);
            await AddData(port, PathPorts);
        }
        public async Task<List<T>> ReadData<T>(string path)
        {
            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var dataJson = await ReadData(stream);
                return JsonConvert.DeserializeObject<List<T>>(dataJson);
            }
        }
        public async Task AddData<T>(T data, string path)
        {
            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var dataJson = await ReadData(stream);
                var datas = JsonConvert.DeserializeObject<List<T>>(dataJson);
                if (datas == null)
                {
                    datas = new List<T>();
                }
                foreach (var oneData in datas)
                {
                    if (oneData.ToString().Equals(data.ToString()) || oneData.ToString() == data.ToString()) ////ok?
                    {
                        return;
                    }
                }
                datas.Add(data);
                await WriteData(path, datas, stream);
            }
        }
        private async Task<string> ReadData(FileStream stream)
        {
            var sb = new StringBuilder();
            var count = 256;
            var buffer = new byte[count];
            while (true)
            {
                var realCount = await stream.ReadAsync(buffer, 0, count);
                sb.Append(Encoding.Default.GetString(buffer, 0, realCount));
                if (realCount < count)
                {
                    break;
                }
            }
            return sb.ToString();
        }
        private async Task WriteData<T>(string path, List<T> data, FileStream stream)
        {
            stream.SetLength(0);
            var dataJson = JsonConvert.SerializeObject(data);
            var buffer = Encoding.Default.GetBytes(dataJson);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
        public async Task WriteData<T>(string path, List<T> data)
        {
            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await WriteData(path, data, stream);
            }
        }
    }
}
