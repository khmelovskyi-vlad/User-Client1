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
            //stupidServer = new StupidServer();
            secondWindowServer = new SecondWindowServer();
            //stupidServer.Run();
            //Process myProcess = new Process();
            //myProcess.StartInfo.UseShellExecute = true;
            //myProcess.StartInfo.FileName = "User-Client";
            //myProcess.StartInfo.Arguments = "1";
            //myProcess.Start();
            //Thread.Sleep(5000);
        }
        private Communication communication;
        //private StupidServer stupidServer;
        private SecondWindowServer secondWindowServer;
        private string TypeChat;
        public void Run()
        {
            //Interlocked
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
                    if (line == "?/send" || line == "?/download" || line == "?/change" || line == "?/invite" || line == "?/delete user"
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
                                }
                                break;
                            case "?/invite":
                                if (TypeChat == "pg" || TypeChat == "ug" || TypeChat == "sg")
                                {
                                    InvitePerson();
                                }
                                break;
                            case "?/delete":
                                DeleteUser();
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
                        autoResetMessage.Set();
                    }
                    else
                    {
                        communication.SendMessage(line);
                    }
                }
            }
        }
        private void DeleteUser()
        {
            while (true)
            {
                var userNick = Console.ReadLine();
                if (userNick.Length > 0)
                {
                    communication.SendMessage(userNick);
                    communication.AnswerAndWriteServer();
                    communication.SendMessage("Okey");
                    return;
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
                if (typeNewGroup == "public" || typeNewGroup == "secret")
                {
                    communication.SendMessage(typeNewGroup);
                    communication.AnswerAndWriteServer();
                    while (true)
                    {
                        var nameNewGroup = Console.ReadLine();
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
            communication.AnswerAndWriteServer();
        }
        private void ReceiveFile()
        {
            while (true)
            {
                var fileName = Console.ReadLine();
                if (fileName.Length > 0)
                {
                    communication.SendMessage(fileName);
                    communication.AnswerAndWriteServer();
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
                        communication.AnswerAndWriteServer();
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
                //stupidServer.AnswerServer(message);
                autoResetMessage.WaitOne();
                if (message == "?/delete")
                {
                    communication.SendMessage("?/end");
                }
                else if (message == "?/you left the chat")
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
                //communication.AnswerAndWriteServer();
                var message = communication.data.ToString();
                secondWindowServer.Write(message);
                //stupidServer.AnswerServer(message);
                communication.SendMessage("ok");
            }
        }
    }
}
