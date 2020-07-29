using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class Detector
    {
        public Detector()
        {
        }
        public async Task Run()
        {
            FileMaster fileMaster = new FileMaster();
            var connector = new Connector(fileMaster);
            try
            {
                while (true)
                {
                    await connector.Run();
                    Console.WriteLine("If you want connect again, click Enter");
                    var key = Console.ReadKey(true);
                    if (key.Key != ConsoleKey.Enter)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }

        }
    }
}
