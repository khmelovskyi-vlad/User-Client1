using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace User_Client
{
    class SecondWindowServer
    {
        private AutoResetEvent autoResetEndWrite = new AutoResetEvent(false);
        public AutoResetEvent autoResetCreated = new AutoResetEvent(false);
        private StreamWriter sw;
        public void Run()
        {
            Process pipeClient = new Process();
            pipeClient.StartInfo.FileName = "User-Client.exe";
            pipeClient.StartInfo.Arguments = "1";
            pipeClient.Start();

            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("help", PipeDirection.Out))
            {
                pipeServer.WaitForConnection();
                try
                {
                    using (sw = new StreamWriter(pipeServer))
                    {
                        sw.AutoFlush = true;
                        //sw.AutoFlush = false;
                        autoResetCreated.Set();
                        autoResetEndWrite.WaitOne();
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
            }
        }
        public void Write(string message)
        {
            sw.WriteLine(message);
            if (message == "?/you left the chat")
            {
                autoResetEndWrite.Set();
            }
            //else
            //{
            //    sw.WriteLine(message);
            //}
        }
    }
}
