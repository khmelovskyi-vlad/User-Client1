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

        public string FindPath()
        {
            var allDisks = AllDisk();
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
                else
                {
                    Console.WriteLine("Don`t understand");
                }
            }
        }
        private void BackFolder()
        {
            var (allDirectoriesAndFilesOrDisks, findDisk) = BackFolder(true);
            Console.WriteLine(allDirectoriesAndFilesOrDisks);
            if (findDisk)
            {
                SelectDisk();
            }
        }
        private bool InFolderOrFile()
        {
            Console.WriteLine("Enter name folder of file");
            while (true)
            {
                var line = Console.ReadLine();
                var (allDirectoriesAndFiles, directoryOrFileFount) = InFolderOrFile(line);
                if (directoryOrFileFount)
                {
                    Console.WriteLine($"{allDirectoriesAndFiles}If you want to select a different folder or file, click C else if you want return to previous folder click P");
                    return false;
                }
                else if (allDirectoriesAndFiles == "Redact" && directoryOrFileFount == false)
                {
                    Console.WriteLine("somethingInteresing");
                    return true;
                    //ReadAndSendFile();
                    //SaveFile(data.ToString());
                    //BackFolder(false);
                    //return;
                }
                else if (allDirectoriesAndFiles == "PathTooLongException" && directoryOrFileFount == false)
                {
                    Console.WriteLine($"Name is too long name, write less");
                }
                else if (allDirectoriesAndFiles == "ArgumentException" && directoryOrFileFount == false)
                {
                    Console.WriteLine($"Bed input {line}, try again");
                }
                else
                {
                    Console.WriteLine(allDirectoriesAndFiles);
                }
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
                    Console.WriteLine($"{allDirectoriesAndFiles}If you want to select a different folder or file, click C else if you want return to previous folder click P");
                    return;
                }
                else
                {
                    Console.WriteLine("Have`t this disk, write name your disk");
                }
            }
        }
        public (string allDirectoriesAndFilesOrDisks, bool findDisk) BackFolder(bool withRemove)
        {
            var allDirectoriesAndFilesOrDisks = "";
            bool findDisk = false;
            try
            {
                if (withRemove)
                {
                    adressName = adressName.Remove(adressName.Length - 1);
                }
                adressName = Path.GetDirectoryName(adressName);
                allDirectoriesAndFilesOrDisks = $"{OutPutFoldersAndFiles()}If you want to select a different folder or file, " +
                    $"click C else if you want return to previous folder click P";
                if (withRemove)
                {
                    adressName = $"{adressName}\\";
                }
            }
            catch (ArgumentNullException)
            {
                findDisk = true;
                allDirectoriesAndFilesOrDisks = $"{AllDisk()}Select Disk";
            }
            return (allDirectoriesAndFilesOrDisks, findDisk);
        }
        public (string allDirectoriesAndFiles, bool directoryOrFileFount) InFolderOrFile(string fileName)
        {
            var Redact = CheckToRedact(fileName);
            if (Redact)
            {
                return ("Redact", false);
            }
            var allDirectoriesAndFiles = ($"Select a folder{enter}");
            var saveAdress = adressName;
            try
            {
                var adress = $"{fileName}\\";
                adressName = $"{adressName}{adress}";
                allDirectoriesAndFiles += OutPutFoldersAndFiles();
            }
            catch (PathTooLongException)
            {
                adressName = saveAdress;
                return ("PathTooLongException", false);
            }
            catch (ArgumentException)
            {
                adressName = saveAdress;
                return ("ArgumentException", false);
            }
            catch (Exception ex)
            {
                adressName = saveAdress;
                return ($"Bed input {ex}, try again", false);
            }
            return (allDirectoriesAndFiles, true);
        }
        private bool CheckToRedact(string fileName)
        {
            if (fileName.Length > 4)
            {
                var lastFourChar = fileName.Substring(fileName.Length - 4);
                if (lastFourChar == ".jpg")
                {
                    if (HaveFile(fileName))
                    {
                        adressName = $"{adressName}{fileName}";
                        return true;
                    }
                }
            }
            return false;
        }
        private bool HaveFile(string fileName)
        {
            var allFiles = Directory.GetFiles(adressName);
            var fileNameAdress = $"{adressName}{fileName}";
            if (allFiles.Length != 0)
            {
                foreach (var file in allFiles)
                {
                    if (fileNameAdress == file)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public string ReadFile()
        {
            return $"???{File.ReadAllText(adressName)}";
        }
        public void SaveFile(string data)
        {
            if (data == "???")
            {
                return;
            }
            File.WriteAllText(adressName, data.ToString(), Encoding.Default);
            return;
        }
        public string AllDisk()
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
            var flag = false;
            foreach (var disk in allDisks)
            {
                if (disk == adressName)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                return (OutPutFoldersAndFiles(), flag);
            }
            else
            {
                return ("", flag);
            }
        }
        private string OutPutFoldersAndFiles()
        {
            StringBuilder allDirectoriesAndFiles = new StringBuilder();
            var allDirectories = Directory.GetDirectories(adressName);
            var allFiles = Directory.GetFiles(adressName);
            allDirectoriesAndFiles.Append(enter);
            //var allFiles = allDirectories.Concat(allDocument);
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
