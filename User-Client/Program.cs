using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //var num = 1000;
            //Console.SetCursorPosition(5, 0);
            //Console.WriteLine ($"Vlad{num, 0}");
            //Console.WriteLine($"{Console.BufferHeight}, {Console.BufferWidth}");
            //Console.ReadKey();
            //ManagerUserInteractor managerUserInteractor = new ManagerUserInteractor();
            //managerUserInteractor.FindPath();
            //Console.ReadLine();
            //var firstPath = @"D:\temp\sudaa";
            //var endPath = @"D:\temp\suda\suda";
            //Directory.Move(firstPath, endPath);
            //Console.ReadKey();
            Detector detector = new Detector();
            detector.Run();

        }
    }
}
