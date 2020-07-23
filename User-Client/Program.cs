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
    class Program
    {
        public static bool TryTo(string path, int milliSecondMax = Timeout.Infinite)
        {
            bool result = false;
            DateTime dateTimestart = DateTime.Now;
            Tuple<AutoResetEvent, FileSystemWatcher> tuple = null;

            while (true)
            {
                try
                {
                    using (var file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        //var writeBuf = Encoding.Default.GetBytes("test");
                        //file.Write(writeBuf, 0, writeBuf.Length);

                        var buffer = new byte[256];
                        var s = file.Read(buffer, 0, buffer.Length);
                        var o = Encoding.Default.GetString(buffer, 0, s);
                        result = true;
                    }
                    File.SetLastWriteTime(path, DateTime.Now);
                        break;
                }
                catch (IOException ex)
                {
                    // Init only once and only if needed. Prevent against many instantiation in case of multhreaded 
                    // file access concurrency (if file is frequently accessed by someone else). Better memory usage.
                    if (tuple == null)
                    {
                        var autoResetEvent = new AutoResetEvent(true);
                        var fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(path))
                        {
                            EnableRaisingEvents = true
                        };

                        fileSystemWatcher.Changed +=
                            (o, e) =>
                            {
                                if (Path.GetFullPath(e.FullPath) == Path.GetFullPath(path))
                                {
                                    autoResetEvent.Set();
                                }
                            };

                        tuple = new Tuple<AutoResetEvent, FileSystemWatcher>(autoResetEvent, fileSystemWatcher);
                    }

                    int milliSecond = Timeout.Infinite;
                    if (milliSecondMax != Timeout.Infinite)
                    {
                        milliSecond = (int)(DateTime.Now - dateTimestart).TotalMilliseconds;
                        if (milliSecond >= milliSecondMax)
                        {
                            result = false;
                            break;
                        }
                    }

                    tuple.Item1.WaitOne(milliSecond);
                }
            }

            if (tuple != null && tuple.Item1 != null) // Dispose of resources now (don't wait the GC).
            {
                tuple.Item1.Dispose();
                tuple.Item2.Dispose();
            }

            return result;
        }
        private static void TestPipeMessenger()
        {
            Process pipeClient = new Process();
            pipeClient.StartInfo.FileName = "User-Client.exe";
            pipeClient.StartInfo.Arguments = "1";
            pipeClient.Start();

            using (NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("testpipe", PipeDirection.Out))
            {
                Console.WriteLine("NamedPipeServerStream object created.");

                // Wait for a client to connect
                Console.Write("Waiting for client connection...");
                pipeServer.WaitForConnection();

                Console.WriteLine("Client connected.");
                try
                {
                    // Read user input and send that to the client process.
                    using (StreamWriter sw = new StreamWriter(pipeServer))
                    {
                        sw.AutoFlush = true;
                        while (true)
                        {
                            Console.Write("Enter text: ");
                            var line = Console.ReadLine();
                            sw.WriteLine(line);
                            if (line == "all")
                            {
                                break;
                            }
                        }
                    }
                }
                // Catch the IOException that is raised if the pipe is broken
                // or disconnected.
                catch (IOException e)
                {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
            }
        }
        private static void TestPipeClient(string[] args)
        {
            using (NamedPipeClientStream pipeClient =
                new NamedPipeClientStream(".", "testpipe", PipeDirection.In))
            {

                // Connect to the pipe or wait until the pipe is available.
                Console.Write("Attempting to connect to pipe...");
                pipeClient.Connect();

                Console.WriteLine("Connected to pipe.");
                Console.WriteLine("There are currently {0} pipe server instances open.",
                   pipeClient.NumberOfServerInstances);
                using (StreamReader sr = new StreamReader(pipeClient))
                {
                    // Display the read text to the console
                    string temp;
                    while ((temp = sr.ReadLine()) != null)
                    {
                        Console.WriteLine("Received from server: {0}", temp);
                    }
                }
            }
            Console.Write("Press Enter to continue...");
            Console.ReadLine();
        }
        static void Main(string[] args)
        {
            //if (args.Length == 0)
            //{
            //    Console.ReadKey();
            //    TestPipeMessenger();
            //}
            //else if (args[0] == "1")
            //{
            //    TestPipeClient(args);
            //}
            //TryTo(@"D:\temp\ok3\test2.txt");
            //Console.ReadKey();
            if (args.Length != 0)
            {
                if (args[0] == "1")
                {
                    StupidConnector stupidConnector = new StupidConnector();
                    stupidConnector.Run();
                }
                //add other options here and below              
            }
            else
            {
                Detector detector = new Detector();
                detector.Run();
            }
            //if (args.Length != 0)
            //{
            //    if (args[0] == "1")
            //    {
            //        StupidConnector stupidConnector = new StupidConnector();
            //        stupidConnector.Run();
            //        Console.WriteLine("kuku");
            //        AlternatePathOfExecution();
            //    }
            //    //add other options here and below              
            //}
            //else
            //{
            //    StupidServer stupidServer = new StupidServer();
            //    stupidServer.Run();
            //    NormalPathOfExectution();
            //}


            //CreateProces();
            //Console.WriteLine("lol");
            //Console.ReadKey();
            ////var num = 1000;
            ////Console.SetCursorPosition(5, 0);
            ////Console.WriteLine ($"Vlad{num, 0}");
            ////Console.WriteLine($"{Console.BufferHeight}, {Console.BufferWidth}");
            ////Console.ReadKey();
            ////ManagerUserInteractor managerUserInteractor = new ManagerUserInteractor();
            ////managerUserInteractor.FindPath();
            ////Console.ReadLine();
            ////var firstPath = @"D:\temp\sudaa";
            ////var endPath = @"D:\temp\suda\suda";
            ////Directory.Move(firstPath, endPath);
            ////Console.ReadKey();
            //Thread myThread;
            //Random rnd = new Random();

            //for (int i = 0; i < numThreads; i++)
            //{
            //    myThread = new Thread(new ThreadStart(MyThreadProc));
            //    myThread.Name = String.Format("Thread{0}", i + 1);

            //    //Wait a random amount of time before starting next thread.
            //    Thread.Sleep(rnd.Next(0, 1000));
            //    myThread.Start();
            //}
            //Console.ReadKey();

            //Detector detector = new Detector();
            //detector.Run();

        }
        private static void CreateProces()
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
            {
                UseShellExecute = true,
                //CreateNoWindow = true,
                //WindowStyle = ProcessWindowStyle.Maximized
            };
            //foreach (Process process in Process.GetProcesses())
            //{
            //    // выводим id и имя процесса
            //    Console.WriteLine($"ID: {process.Id}  Name: {process.ProcessName}");
            //}
            Process p = Process.Start(psi);
            //StreamWriter sw = p.StandardInput;
            //StreamReader sr = p.StandardOutput;
            //sw.WriteLine("Hello world!");
            //sw.Write("Hello world!");
            //Console.WriteLine(sr.Read());
            //sr.Close();
        }
        private static void NormalPathOfExectution()
        {
            Console.WriteLine("Doing something here");
            //need one of these for each additional console window
            Task.Run(() => Process.Start("User-Client", "1"));
            Console.WriteLine("Doing something here");
            Console.ReadLine();

        }
        private static void AlternatePathOfExecution()
        {
            Console.WriteLine("Write something different on other Console");
            Console.ReadLine();
        }
        private static int usingResource = 0;

        private const int numThreadIterations = 5;
        private const int numThreads = 10;

        private static void MyThreadProc()
        {
            for (int i = 0; i < numThreadIterations; i++)
            {
                UseResource();

                //Wait 1 second before next attempt.
                Thread.Sleep(1000);
            }
        }

        //A simple method that denies reentrancy.
        static bool UseResource()
        {
            //0 indicates that the method is not in use.
            if (0 == Interlocked.Exchange(ref usingResource, 1))
            {
                Console.WriteLine("{0} acquired the lock", Thread.CurrentThread.Name);

                //Code to access a resource that is not thread safe would go here.

                //Simulate some work
                Thread.Sleep(500);

                Console.WriteLine("{0} exiting lock", Thread.CurrentThread.Name);

                //Release the lock
                Interlocked.Exchange(ref usingResource, 0);
                return true;
            }
            else
            {
                Console.WriteLine("   {0} was denied the lock", Thread.CurrentThread.Name);
                return false;
            }
        }
    }
}
