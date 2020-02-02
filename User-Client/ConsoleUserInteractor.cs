using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class ConsoleUserInteractor : IUserInteractor
    {
        public UserAction AskAction()
        {
            while (true)
            {
                Console.WriteLine("If you want to select a different folder or file, click Enter");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    return UserAction.SelectFolder;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    return UserAction.BackFolder;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return UserAction.Close;
                }
            }
        }

        public void ShowFolderFiles(IEnumerable<string> files)
        {
            if (files.Any())
            {
                Console.WriteLine();
                Console.WriteLine("This directory has following files: ");
                foreach (var file in files)
                {
                    Console.WriteLine($"{Path.GetFileName(file)}, ");
                }
            }
            Console.WriteLine();
        }

        public void ShowFolderForlders(IEnumerable<string> folders)
        {
            if (folders.Any())
            {
                Console.WriteLine();
                Console.WriteLine("This directory has following directories: ");
                foreach (var directory in folders)
                {
                    Console.WriteLine($"{Path.GetFileName(directory)}, ");
                }
            }
        }
    }
}
