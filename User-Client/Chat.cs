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
        public async Task Run()
        {
            //Interlocked
            await communication.SendMessage("okey");
            Task.Run(() => secondWindowServer.Run());
            secondWindowServer.autoResetCreated.WaitOne();
            await communication.AnswerServer();
            TypeChat = communication.data.ToString();
            await communication.SendMessage("ok");
            await communication.AnswerAndWriteServer();
            await communication.SendMessage("ok");
            await WriteMessages();
            AnswerUsers();
            await Communicate();
        }
        private async Task Communicate()
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Length > 0)
                {
                    if (line == "?/send" || line == "?/download" || line == "?/change" || line == "?/invite" || line == "?/delete"
                        || line == "?/leave a group" || line == "?/end")
                    {
                        autoResetMessage.WaitOne();
                        await communication.SendMessage(line);
                        switch (line)
                        {
                            case "?/send":
                                await SendFile();
                                break;
                            case "?/download":
                                await ReceiveFile();
                                break;
                            case "?/change":
                                if (TypeChat == "pp" || TypeChat == "ch")
                                {
                                    await ChangeTypeGroup();
                                    break;
                                }
                                autoResetMessage.Set();
                                continue;
                            case "?/invite":
                                if (TypeChat == "pg" || TypeChat == "ug" || TypeChat == "sg")
                                {
                                    await InviteOrDeleteUser();
                                    break;
                                }
                                autoResetMessage.Set();
                                continue;
                            case "?/delete":
                                await InviteOrDeleteUser();
                                break;
                            case "?/leave a group":
                                var successLeave = await LeaveGroup();
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
                        await communication.SendMessage("okey");
                        await WriteMessages();
                        autoResetMessage.Set();
                    }
                    else
                    {
                        await communication.SendMessage(line);
                    }
                }
            }
        }
        private async Task InviteOrDeleteUser()
        {
            while (true)
            {
                var userNick = Console.ReadLine();
                if (userNick.Length > 0)
                {
                    await communication.SendMessage(userNick);
                    await communication.AnswerAndWriteToSecndWindow(secondWindowServer);
                    return;
                }
            }
        }
        private async Task ChangeTypeGroup()
        {
            while (true)
            {
                var typeNewGroup = Console.ReadLine();
                if (typeNewGroup == "public" || typeNewGroup == "secret")
                {
                    await communication.SendMessage(typeNewGroup);
                    await communication.AnswerAndWriteToSecndWindow(secondWindowServer);
                    while (true)
                    {
                        var nameNewGroup = Console.ReadLine();
                        if (nameNewGroup.Length > 0)
                        {
                            await communication.SendMessage(nameNewGroup);
                            await communication.AnswerAndWriteToSecndWindow(secondWindowServer);
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
                    await communication.SendMessage("End");
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
        private async Task SendFile()
        {
            ManagerUserInteractor managerUserInteractor = new ManagerUserInteractor();
            var path = managerUserInteractor.FindPath();
            if (path == "?/escape")
            {
                await communication.SendMessage(path);
                return;
            }
            var fileName = Path.GetFileName(path);
            await communication.SendFile(path, fileName);
            await communication.AnswerAndWriteToSecndWindow(secondWindowServer);
        }
        private async Task ReceiveFile()
        {
            while (true)
            {
                var fileName = Console.ReadLine();
                if (fileName.Length > 0)
                {
                    await communication.SendMessage(fileName);
                    await communication.AnswerAndWriteToSecndWindow(secondWindowServer);
                    await CheckFindedFile(fileName);
                    return;
                }
            }
        }
        private async Task CheckFindedFile(string fileName)
        {
            if (communication.data.ToString() == "Finded")
            {
                await FindNameAndRecive();
            }
            else if (communication.data.ToString().Substring(0, 9) == "Have some")
            {
                while (true)
                {
                    var dateFile = Console.ReadLine();
                    if (dateFile.Length > 0)
                    {
                        await communication.SendMessage(dateFile);
                        await communication.AnswerAndWriteToSecndWindow(secondWindowServer);
                        if (communication.data.ToString() == "Finded")
                        {
                            await FindNameAndRecive();
                        }
                        return;
                    }
                }
            }
            async Task FindNameAndRecive()
            {
                fileName = CheckFile(fileName);
                await communication.SendMessage("ok");
                await communication.ReciveFile($@"D:\temp\User\{fileName}");
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
        private async Task<bool> LeaveGroup()
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Length > 0)
                {
                    await communication.SendMessage(line);
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
        private async Task AnswerUsers()
        {
            while (true)
            {
                await communication.AnswerServer();
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
        private async Task WriteMessages()
        {
            await communication.AnswerServer();
            await communication.SendMessage("ok");
            var count = Convert.ToInt32(communication.data.ToString());
            for (int i = 0; i < count; i++)
            {
                await communication.AnswerServer();
                secondWindowServer.Write(communication.data.ToString());
                await communication.SendMessage("ok");
            }
        }
    }
}
