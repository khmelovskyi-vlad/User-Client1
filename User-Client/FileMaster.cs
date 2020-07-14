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
        public void AddPortAndIP(object IP, object port)
        {
            AddData(IP, PathIps);
            AddData(port, PathPorts);
        }
        public object ReadData(string path)
        {
            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var dataJson = ReadData(stream);
                return JsonConvert.DeserializeObject<List<object>>(dataJson);
            }
        }
        public List<UserNicknameAndPassword> ReadDataToUser(string path)
        {
            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var dataJson = ReadData(stream);
                return JsonConvert.DeserializeObject<List<UserNicknameAndPassword>>(dataJson);
            }
        }
        public void AddData(object data, string path)
        {
            using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var dataJson = ReadData(stream);
                var datas = JsonConvert.DeserializeObject<List<object>>(dataJson);
                if (datas == null)
                {
                    datas = new List<object>();
                }
                foreach (var oneData in datas)
                {
                    if (oneData.ToString().Equals(data.ToString()) || oneData.ToString() == data.ToString()) ////ok?
                    {
                        return;
                    }
                }
                datas.Add(data);
                WriteData(path, datas, stream);
            }
        }
        private string ReadData(FileStream stream)
        {
            var sb = new StringBuilder();
            var count = 256;
            var buffer = new byte[count];
            while (true)
            {
                var realCount = stream.Read(buffer, 0, count);
                sb.Append(Encoding.Default.GetString(buffer, 0, realCount));
                if (realCount < count)
                {
                    break;
                }
            }
            return sb.ToString();
        }
        private void WriteData(string path, object data, FileStream stream)
        {
            stream.SetLength(0);
            var dataJson = JsonConvert.SerializeObject(data);
            var buffer = Encoding.Default.GetBytes(dataJson);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
