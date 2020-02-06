﻿using System;
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
        private const string message = "If you want to select a different folder or file, click C\n\r" +
                        "If you want return to previous folder click P\r\n" +
                        "If you want to send this file, click S";
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
                else if (key.Key == ConsoleKey.S)
                {
                    if (CheckSend(true) == "send")
                    {
                        return adressName;
                    }
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return "?";
                }
                else
                {
                    Console.WriteLine("Don`t understand");
                }
            }
        }
        private (bool, string) CheckPath(bool needRamove)
        {
            string path;
            if (needRamove)
            {
                path = adressName.Remove(adressName.Length - 1);
            }
            else
            {
                path = adressName;
            }
            var canSend = File.Exists(path);
            if (canSend)
            {
                Console.WriteLine("We can send this file, click O if you want to do it");
            }
            else
            {
                Console.WriteLine("Can`t send this file, find else");
            }
            return (canSend, path);
            //var canSend = File.Exists(adressName);
            //return canSend;
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
                    Console.WriteLine($"{allDirectoriesAndFiles}{message}");
                    return false;
                }
                else if (allDirectoriesAndFiles == "Send" && directoryOrFileFount == false)
                {
                    Console.WriteLine("File will be send");
                    return true;
                    //ReadAndSendFile();
                    //SaveFile(data.ToString());
                    //BackFolder(false);
                    //return;
                }
                else if (allDirectoriesAndFiles == "Have file" && directoryOrFileFount == false)
                {
                    Console.WriteLine(message);
                    return false;
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
                    Console.WriteLine($"{allDirectoriesAndFiles}{message}");
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
                allDirectoriesAndFilesOrDisks = $"{OutPutFoldersAndFiles()}{message}";
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
            //var Redact = CheckToRedact(fileName);
            //if (Redact)
            //{
            //    return ("Redact", false);
            //}
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
            catch (IOException ex)
            {
                var message = CheckSend(true);
                if (message == "send")
                {
                    return ("Send", false);
                }
                else if (message == "have")
                {
                    adressName = saveAdress;
                    return ("Have file", false);
                }
                else
                {
                    adressName = saveAdress;
                    return ($"Bed input {ex}, try again", false);
                }
            }
            catch (Exception ex)
            {
                adressName = saveAdress;
                return ($"Bed input {ex}, try again", false);
            }
            return (allDirectoriesAndFiles, true);
        }
        private string CheckSend(bool needRemove)
        {
            var (canSend, path) = CheckPath(needRemove);
            if (canSend)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.O)
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
        //private bool CheckToRedact(string fileName)
        //{
        //    if (fileName.Length > 4)
        //    {
        //        var lastFourChar = fileName.Substring(fileName.Length - 4);
        //        if (lastFourChar == ".jpg")
        //        {
        //            if (HaveFile(fileName))
        //            {
        //                adressName = $"{adressName}{fileName}";
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
        //private bool HaveFile(string fileName)
        //{
        //    var allFiles = Directory.GetFiles(adressName);
        //    var fileNameAdress = $"{adressName}{fileName}";
        //    if (allFiles.Length != 0)
        //    {
        //        foreach (var file in allFiles)
        //        {
        //            if (fileNameAdress == file)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
        //public string ReadFile()
        //{
        //    return $"???{File.ReadAllText(adressName)}";
        //}
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
