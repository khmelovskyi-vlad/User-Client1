using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace User_Client
{
    class Chat
    {
        public Chat(Communication communication)
        {
            this.communication = communication;
            stupidServer = new StupidServer();
            stupidServer.Run();
            Process myProcess = new Process();
            myProcess.StartInfo.UseShellExecute = true;
            myProcess.StartInfo.FileName = "User-Client";
            myProcess.StartInfo.Arguments = "1";
            myProcess.Start();
            //Thread.Sleep(5000);
        }
        private Communication communication;
        private int check = 0;
        private AutoResetEvent autoResetEvent = new AutoResetEvent(true);
        private StupidServer stupidServer;
        private int block = 0;
        public void Run()
        {
            //Interlocked
            communication.AnswerAndWriteServer();
            communication.SendMessage("ok");
            WriteMessages();
            Task.Run(() => AnswerUsers());
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Length > 0)
                {
                    if (line == "?/send")
                    {
                        //Interlocked check
                        block = 1;
                        communication.SendMessage(line);
                        SendFile();
                        block = 0;
                        autoResetEvent.Set();
                        continue;
                    }
                    else if (line == "?/download")
                    {
                        block = 1;
                        communication.SendMessage(line);
                        ReciveFile();
                        block = 0;
                        autoResetEvent.Set();
                        continue;
                    }
                    else if (line == "?/change")
                    {
                        block = 1;
                        communication.SendMessage(line);
                        ChangeTypeGroup();
                        block = 0;
                        autoResetEvent.Set();
                        continue;
                    }
                    else if (line == "?/invite")
                    {
                        block = 1;
                        communication.SendMessage(line);
                        InvitePerson();
                        block = 0;
                        autoResetEvent.Set();
                        continue;
                    }
                    communication.SendMessage(line);
                    if (line == "?/end")
                    {
                        EndAnswer = true;
                        return;
                    }
                    else if (line == "?/leave a group")
                    {
                        return;

                        //var successLeave = LeaveGroup();
                        //if (successLeave)
                        //{
                        //    return;
                        //}
                    }
                }
            }
        }
        private void InvitePerson()
        {
            while (true)
            {
                var typeNewGroup = Console.ReadLine();
                if (typeNewGroup.Length > 0)
                {
                    communication.SendMessage(typeNewGroup);
                    if (typeNewGroup == "?")
                    {
                        return;
                    }
                    communication.AnswerAndWriteServer();
                    return;
                }
            }
        }
        private void ChangeTypeGroup()
        {
            while (true)
            {
                var typeNewGroup = Console.ReadLine();
                if (typeNewGroup == "?")
                {
                    communication.SendMessage(typeNewGroup);
                    return;
                }
                else if (typeNewGroup == "public" || typeNewGroup == "secret")
                {
                    communication.SendMessage(typeNewGroup);
                    communication.AnswerAndWriteServer();
                }
                else
                {
                    Console.WriteLine("Bed input");
                    continue;
                }
                while (true)
                {
                    var nameNewGroup = Console.ReadLine();
                    if (nameNewGroup == "?")
                    {
                        communication.SendMessage(nameNewGroup);
                        return;
                    }
                    if (nameNewGroup.Length > 0)
                    {
                        communication.SendMessage(nameNewGroup);
                        communication.AnswerAndWriteServer();
                        if (communication.data.ToString() == $"New group have {typeNewGroup} type and name {nameNewGroup}")
                        {
                            return;
                        }
                        Console.WriteLine("Bed input");
                    }
                }
            }
        }
        private void SendFile()
        {
            ManagerUserInteractor managerUserInteractor = new ManagerUserInteractor();
            var path = managerUserInteractor.FindPath();
            if (path == "?")
            {
                communication.SendMessage(path);
                return;
            }
            var fileName = Path.GetFileName(path);
            communication.SendFile(path, fileName);
        }
        private void ReciveFile()
        {
            while (true)
            {
                var nameFile = Console.ReadLine();
                if (nameFile.Length > 0)
                {
                    communication.SendMessage(nameFile);
                    communication.AnswerAndWriteServer();
                    if (communication.data.ToString() == "Finded")
                    {
                        nameFile = CheckFile(nameFile);
                        communication.SendMessage("ok");
                        communication.ReciveFile($@"D:\temp\User\{nameFile}");
                    }
                    else if (communication.data.ToString()[0] == 'H')
                    {
                        while (true)
                        {
                            var dateFile = Console.ReadLine();
                            if (dateFile.Length > 0)
                            {
                                communication.SendMessage(dateFile);
                                communication.AnswerAndWriteServer();
                                if (communication.data.ToString() == "Finded")
                                {
                                    nameFile = CheckFile(nameFile);
                                    communication.SendMessage("ok");
                                    communication.ReciveFile($@"D:\temp\User\{nameFile}");
                                }
                                return;
                            }
                        }
                    }
                    return;
                }
            }
        }
        private string CheckFile(string nameFile)
        {
            var filesPaths = Directory.GetFiles(@"D:\temp\User");
            int i = 0;
            while (true)
            {
                var haveFile = false;
                foreach (var filePath in filesPaths)
                {
                    var file = Path.GetFileName(filePath);
                    if (file == nameFile)
                    {
                        haveFile = true;
                        break;
                    }
                }
                if (haveFile == false)
                {
                    return nameFile;
                }
                if (i != 0)
                {
                    var iCount = i.ToString().Length;
                    nameFile = nameFile.Remove(0, iCount);
                }
                i++;
                nameFile = $"{i.ToString()}{nameFile}";
            }
        }
        private void DeleteUser()
        {

        }
        private void ReciveChatMessages()
        {

        }
        private bool LeaveGroup()
        {
            communication.AnswerAndWriteServer();
            while (true)
            {
                var line = Console.ReadLine();
                if (check == 1)
                {
                    return false;
                }
                if (line.Length > 0)
                {
                    communication.SendMessage(line);
                    communication.AnswerAndWriteServer();
                    if (communication.data.ToString() == "You leave a chat")
                    {
                        return true;
                    }
                    else if (communication.data.ToString() == "Ok, you did not to leave chat")
                    {
                        return false;
                    }
                }
            }
        }
        private bool EndAnswer = false;
        private void AnswerUsers()
        {
            while (true)
            {
                autoResetEvent.WaitOne();
                communication.AnswerServer();
                var message = communication.data.ToString();
                stupidServer.AnswerServer(message);
                if (block == 1)
                {
                    autoResetEvent.WaitOne();
                }
                if (message == "?/delete user")
                {
                    communication.SendMessage("?/end");
                    check = 1;
                }
                if (EndAnswer)
                {
                    return;
                }
                autoResetEvent.Set();
            }
        }
        private void WriteMessages()
        {
            communication.AnswerServer();
            communication.SendMessage("ok");
            var count = Convert.ToInt32(communication.data.ToString());
            for (int i = 0; i < count; i++)
            {
                communication.AnswerAndWriteServer();
                var message = communication.data.ToString();
                stupidServer.AnswerServer(message);
                communication.SendMessage("ok");
            }
        }
    }
}
