using System;
using System.Collections.Generic;
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
            Detector detector = new Detector();
            detector.Run();

        }
    }
}
