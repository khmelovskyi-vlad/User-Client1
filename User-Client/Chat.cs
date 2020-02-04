using System;
using System.Collections.Generic;
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
        }
        Communication communication;
        private int check = 0;
        AutoResetEvent autoResetEvent = new AutoResetEvent(true);
         
        private int block = 0;
        public void Run()
        {

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
                    communication.SendMessage(line);
                    if (line == "?/end")
                    {
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
        private void SendFile()
        {
            ManagerUserInteractor managerUserInteractor = new ManagerUserInteractor();
            var path = managerUserInteractor.FindPath();
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
        private void AnswerUsers()
        {
            while (true)
            {
                autoResetEvent.WaitOne();
                communication.AnswerAndWriteServer();
                if (block == 1)
                {
                    autoResetEvent.WaitOne();
                }
                if (communication.data.ToString() == "?/delete user")
                {
                    communication.SendMessage("?/end");
                    check = 1;
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
                communication.SendMessage("ok");
            }
        }
    }
}
