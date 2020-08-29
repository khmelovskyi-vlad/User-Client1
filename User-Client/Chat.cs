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
            Task.Run(() => secondWindowServer.Run());
            secondWindowServer.autoResetCreated.WaitOne();
            TypeChat = await communication.ListenServer();
            await communication.ListenServerWrite();
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
                    if (line == "?/")
                    {
                        autoResetMessage.WaitOne();
                        await communication.SendMessage(line);
                        var mode = Console.ReadLine();
                        await communication.SendMessageListenServerWrite(mode);
                        await RunModeSession(mode);
                        
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
        private async Task RunModeSession(string mode)
        {
            switch (mode)
            {
                case "?/send":
                    await SendFile();
                    return;
                case "?/download":
                    await ReceiveFile();
                    return;
                case "?/change":
                    if (TypeChat == "pp" || TypeChat == "ch")
                    {
                        await ChangeTypeGroup();
                    }
                    return;
                case "?/invite":
                    if (TypeChat == "pg" || TypeChat == "ug" || TypeChat == "sg")
                    {
                        await InviteOrDeleteUser();
                    }
                    return;
                case "?/delete":
                    await InviteOrDeleteUser();
                    return;
                case "?/leave a group":
                    var successLeave = await LeaveGroup();
                    if (successLeave)
                    {
                        autoResetMessage.Set();
                        throw new OperationCanceledException();
                    }
                    return;
                case "?/end":
                    autoResetMessage.Set();
                    throw new OperationCanceledException();
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
                    await communication.ListenServerWriteToSecondWindow(secondWindowServer);
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
                    var message = await communication.ListenServerWriteToSecondWindow(secondWindowServer);
                    while (true)
                    {
                        var nameNewGroup = Console.ReadLine();
                        if (nameNewGroup.Length > 0)
                        {
                            await communication.SendMessage(nameNewGroup);
                            message = await communication.ListenServerWriteToSecondWindow(secondWindowServer);
                            if (message == $"New group have {typeNewGroup} type and name {nameNewGroup}")
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
                    Console.WriteLine("Bed input");
                    Console.WriteLine("If you want to change the group type, press 'Enter'");
                    if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine("Write need type");
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
            await communication.SendMessage(fileName);
            await communication.SendFile(path);
            await communication.ListenServerWriteToSecondWindow(secondWindowServer);
        }
        private async Task ReceiveFile()
        {
            while (true)
            {
                var fileName = Console.ReadLine();
                if (fileName.Length > 0)
                {
                    await communication.SendMessage(fileName);
                    await CheckFindedFile(fileName);
                    return;
                }
            }
        }
        private async Task CheckFindedFile(string fileName)
        {
            var message = await communication.ListenServerWriteToSecondWindow(secondWindowServer);
            if (message == "Finded")
            {
                await FindNameAndRecive();
            }
            else if (message == "Have some files")
            {
                message = await communication.ListenServerWriteToSecondWindow(secondWindowServer);
                while (true)
                {
                    var dateFile = Console.ReadLine();
                    if (dateFile.Length > 0)
                    {
                        await communication.SendMessage(dateFile);
                        message = await communication.ListenServerWriteToSecondWindow(secondWindowServer);
                        if (message == "Finded")
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
                        await communication.ListenServerWriteToSecondWindow(secondWindowServer);
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
                var message = await communication.ListenServer();
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
            var count = Convert.ToInt32(await communication.ListenServer());
            for (int i = 0; i < count; i++)
            {
                secondWindowServer.Write(await communication.ListenServer());
            }
        }
    }
}
