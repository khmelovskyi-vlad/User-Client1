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
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                if (args[0] == "1")
                {
                    SecondWindowClien secondWindowClien = new SecondWindowClien();
                    secondWindowClien.Run();
                }            
            }
            else
            {
                Detector detector = new Detector();
                detector.Run();
            }
        }
    }
}
