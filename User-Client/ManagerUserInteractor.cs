using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class ManagerUserInteractor
    {
        public ManagerUserInteractor()
        {
        }
        public string adressName { get; set; }
        private string[] allDisks;
        private const string enter = "\r\n";
        private const string message = "If you want to select a different folder or file, click 'C'\n\r" +
                        "If you want to return to the previous folder, click 'P'\r\n" +
                        "If you want to send this file, click 'S'";
        public string FindPath()
        {
            var allDisks = GetAllDisk();
            Console.WriteLine($"{allDisks}\r\nSelect a disc\r\nWrite name your disk");
            SelectDisk();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.C)
                {
                    if (InFolderOrFile())
                    {
                        return adressName;
                    }
                }
                else if (key.Key == ConsoleKey.P)
                {
                    BackFolder();
                }
                else if (key.Key == ConsoleKey.S)
                {
                    if (CheckSend() == "send")
                    {
                        return adressName;
                    }
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return "?/escape";
                }
                else
                {
                    Console.WriteLine("Don`t understand");
                }
            }
        }
        private void BackFolder()
        {
            var allDirectoriesAndFilesOrDisks = "";
            var haveEx = false;
            try
            {
                adressName = adressName.Remove(adressName.Length - 1);
                adressName = Path.GetDirectoryName(adressName);
                allDirectoriesAndFilesOrDisks = $"{OutPutFoldersAndFiles()}{message}";
                adressName = $"{adressName}\\";
            }
            catch (ArgumentNullException)
            {
                allDirectoriesAndFilesOrDisks = $"{GetAllDisk()}Select Disk";
                haveEx = true;
            }
            Console.WriteLine(allDirectoriesAndFilesOrDisks);
            if (haveEx)
            {
                SelectDisk();
            }
        }
        private void SelectDisk()
        {
            while (true)
            {
                var line = Console.ReadLine();
                var (allDirectoriesAndFiles, diskFound) = SelectDisk(line);
                if (diskFound)
                {
                    Console.WriteLine($"{allDirectoriesAndFiles}{message}");
                    return;
                }
                else
                {
                    Console.WriteLine("Have`t this disk, write name your disk");
                }
            }
        }
        private bool InFolderOrFile()
        {
            Console.WriteLine("Enter a name for the folder or file");
            while (true)
            {
                var fileName = Console.ReadLine();
                var allDirectoriesAndFiles = ($"Select a folder{enter}");
                var saveAdress = adressName;
                try
                {
                    var adress = $"{fileName}\\";
                    adressName = $"{adressName}{adress}";
                    allDirectoriesAndFiles += OutPutFoldersAndFiles();
                    Console.WriteLine($"{allDirectoriesAndFiles}{message}");
                    return false;
                }
                catch (PathTooLongException)
                {
                    adressName = saveAdress;
                    Console.WriteLine($"The name is too long, write less");
                }
                catch (ArgumentException)
                {
                    adressName = saveAdress;
                    Console.WriteLine($"Bed input {fileName}, try again");
                }
                catch (IOException)
                {
                    var type = CheckSend();
                    if (type == "send")
                    {
                        Console.WriteLine("The file will be sent");
                        return true;
                    }
                    else if (type == "have")
                    {
                        adressName = saveAdress;
                        Console.WriteLine(message);
                        return false;
                    }
                    else
                    {
                        adressName = saveAdress;
                        //return ($"Bed input {ex}, try again", false);
                    }
                }
                catch (Exception)
                {
                    adressName = saveAdress;
                    Console.WriteLine("Bed input, try again");
                    //return ($"Bed input {ex}, try again", false);
                }
            }
        }
        private string CheckSend()
        {
            var (canSend, path) = CheckPath();
            if (canSend)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.S)
                {
                    adressName = path;
                    return "send";
                }
                else
                {
                    Console.WriteLine("No problems");
                    return "have";
                }
            }
            return "don`t have";
        }
        private (bool, string) CheckPath()
        {
            string path = adressName.Remove(adressName.Length - 1);
            var canSend = File.Exists(path);
            if (canSend)
            {
                Console.WriteLine("We can send this file, click 'S' if you want to do it");
            }
            else
            {
                Console.WriteLine("Can`t send this file, find else");
            }
            return (canSend, path);
        }
        public string GetAllDisk()
        {
            StringBuilder allDisksReturn = new StringBuilder();
            allDisksReturn.Append($"You have disks:{enter}");
            allDisks = Directory.GetLogicalDrives();
            foreach (var disk in allDisks)
            {
                allDisksReturn.Append($"{disk}{enter}");
            }
            return allDisksReturn.ToString();
        }
        public (string allDirectoriesAndFiles, bool diskFound) SelectDisk(string line)
        {
            adressName = $"{line}:\\";
            foreach (var disk in allDisks)
            {
                if (disk == adressName)
                {
                    return (OutPutFoldersAndFiles(), true);
                }
            }
            return ("", false);
        }
        private string OutPutFoldersAndFiles()
        {
            StringBuilder allDirectoriesAndFiles = new StringBuilder();
            var allDirectories = Directory.GetDirectories(adressName);
            var allFiles = Directory.GetFiles(adressName);
            allDirectoriesAndFiles.Append(enter);
            if (allDirectories.Length != 0)
            {
                allDirectoriesAndFiles.Append($"This directory has following directories:{enter}");
                foreach (var directory in allDirectories)
                {
                    allDirectoriesAndFiles.Append($"{Path.GetFileName(directory)}{enter}");
                }
            }
            if (allFiles.Length != 0)
            {
                allDirectoriesAndFiles.Append($"This directory has following files:{enter}");
                foreach (var file in allFiles)
                {
                    allDirectoriesAndFiles.Append($"{Path.GetFileName(file)}{enter}");
                }
            }
            return allDirectoriesAndFiles.ToString();
        }
    }
}
