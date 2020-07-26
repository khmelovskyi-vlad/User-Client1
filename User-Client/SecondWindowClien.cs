using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class SecondWindowClien
    {
        public void Run()
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.In))
            {
                pipeClient.Connect();
                
                using (StreamReader sr = new StreamReader(pipeClient))
                {
                    string temp;
                    while ((temp = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(temp);
                    }
                }
            }
        }
    }
}
