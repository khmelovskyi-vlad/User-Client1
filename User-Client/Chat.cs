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
            secondWindowServer = new SecondWindowServer();
        }
        private Communication communication;
        private SecondWindowServer secondWindowServer;
        private string TypeChat;
        public void Run()
        {
            //Interlocked
            communication.SendMessage("okey");
            Task.Run(() => secondWindowServer.Run());
            secondWindowServer.autoResetCreated.WaitOne();
            communication.AnswerServer();
            TypeChat = communication.data.ToString();
            communication.SendMessage("ok");
            communication.AnswerAndWriteServer();
            communication.SendMessage("ok");
            WriteMessages();
            Task.Run(() => AnswerUsers());
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Length > 0)
                {
                    if (line == "?/send" || line == "?/download" || line == "?/change" || line == "?/invite" || line == "?/delete"
                        || line == "?/leave a group" || line == "?/end")
                    {
                        autoResetMessage.WaitOne();
                        communication.SendMessage(line);
                        switch (line)
                        {
                            case "?/send":
                                SendFile();
                                break;
                            case "?/download":
                                ReceiveFile();
                                break;
                            case "?/change":
                                if (TypeChat == "pp" || TypeChat == "ch")
                                {
                                    ChangeTypeGroup();
                                    break;
                                }
                                autoResetMessage.Set();
                                continue;
                            case "?/invite":
                                if (TypeChat == "pg" || TypeChat == "ug" || TypeChat == "sg")
                                {
                                    InviteOrDeleteUser();
                                    break;
                                }
                                autoResetMessage.Set();
                                continue;
                            case "?/delete":
                                InviteOrDeleteUser();
                                break;
                            case "?/leave a group":
                                var successLeave = LeaveGroup();
                                if (successLeave)
                                {
                                    autoResetMessage.Set();
                                    return;
                                }
                                break;
                            case "?/end":
                                autoResetMessage.Set();
                                return;
                        }
                        communication.SendMessage("okey");
                        WriteMessages();
                        autoResetMessage.Set();
                    }
                    else
                    {
                        communication.SendMessage(line);
                    }
                }
            }
        }
        private void InviteOrDeleteUser()
        {
            while (true)
            {
                var userNick = Console.ReadLine();
                if (userNick.Length > 0)
                {
                    communication.SendMessage(userNick);
                    communication.AnswerAndWriteToSecndWindow(secondWindowServer);
                    return;
                }
            }
        }
        private void ChangeTypeGroup()
        {
            while (true)
            {
                var typeNewGroup = Console.ReadLine();
                if (typeNewGroup == "public" || typeNewGroup == "secret")
                {
                    communication.SendMessage(typeNewGroup);
                    communication.AnswerAndWriteToSecndWindow(secondWindowServer);
                    while (true)
                    {
                        var nameNewGroup = Console.ReadLine();
                        if (nameNewGroup.Length > 0)
                        {
                            communication.SendMessage(nameNewGroup);
                            communication.AnswerAndWriteToSecndWindow(secondWindowServer);
                            if (communication.data.ToString() == $"New group have {typeNewGroup} type and name {nameNewGroup}")
                            {
                                ChangeTypeGroup(typeNewGroup);
                                return;
                            }
                            Console.WriteLine("Bed input");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Bed input\n\r" +
                        "If you want to change the group type, press 'Enter'");
                    if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine(communication.data);
                        continue;
                    }
                    Console.WriteLine("Okey, you can write the message");
                    communication.SendMessage("End");
                    return;
                }
            }
        }
        private void ChangeTypeGroup(string typeGroup)
        {
            if (typeGroup == "public")
            {
                TypeChat = "ug";
            }
            else if (typeGroup == "secret")
            {
                TypeChat = "sg";
            }
        }
        private void SendFile()
        {
            ManagerUserInteractor managerUserInteractor = new ManagerUserInteractor();
            var path = managerUserInteractor.FindPath();
            if (path == "?/escape")
            {
                communication.SendMessage(path);
                return;
            }
            var fileName = Path.GetFileName(path);
            communication.SendFile(path, fileName);
            communication.AnswerAndWriteToSecndWindow(secondWindowServer);
        }
        private void ReceiveFile()
        {
            while (true)
            {
                var fileName = Console.ReadLine();
                if (fileName.Length > 0)
                {
                    communication.SendMessage(fileName);
                    communication.AnswerAndWriteToSecndWindow(secondWindowServer);
                    CheckFindedFile(fileName);
                    return;
                }
            }
        }
        private void CheckFindedFile(string fileName)
        {
            if (communication.data.ToString() == "Finded")
            {
                FindNameAndRecive();
            }
            else if (communication.data.ToString().Substring(0, 9) == "Have some")
            {
                while (true)
                {
                    var dateFile = Console.ReadLine();
                    if (dateFile.Length > 0)
                    {
                        communication.SendMessage(dateFile);
                        communication.AnswerAndWriteToSecndWindow(secondWindowServer);
                        if (communication.data.ToString() == "Finded")
                        {
                            FindNameAndRecive();
                        }
                        return;
                    }
                }
            }
            void FindNameAndRecive()
            {
                fileName = CheckFile(fileName);
                communication.SendMessage("ok");
                communication.ReciveFile($@"D:\temp\User\{fileName}");
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
        private bool LeaveGroup()
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Length > 0)
                {
                    communication.SendMessage(line);
                    if (line == "yes")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        private AutoResetEvent autoResetMessage = new AutoResetEvent(true);
        public AutoResetEvent autoResetConnectAgain = new AutoResetEvent(false);
        private void AnswerUsers()
        {
            while (true)
            {
                communication.AnswerServer();
                var message = communication.data.ToString();
                secondWindowServer.Write(message);
                autoResetMessage.WaitOne();
                if (message == "?/you left the chat") //кожен раз створюється строка "?/you left the chat"
                {
                    autoResetConnectAgain.Set();
                    return;
                }
                autoResetMessage.Set();
            }
        }
        private void WriteMessages()
        {
            communication.AnswerServer();
            communication.SendMessage("ok");
            var count = Convert.ToInt32(communication.data.ToString());
            for (int i = 0; i < count; i++)
            {
                communication.AnswerServer();
                secondWindowServer.Write(communication.data.ToString());
                communication.SendMessage("ok");
            }
        }
    }
}
