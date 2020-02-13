using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace User_Client
{
    class Program
    {
        static void Main(string[] args)
        {
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
