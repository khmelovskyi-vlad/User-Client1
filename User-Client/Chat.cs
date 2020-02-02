using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
                    if (line == "?/send file")
                    {
                        FileManager manager = new FileManager(new ConsoleUserInteractor());
                        manager.FileManage();
                    }
                }
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
                communication.AnswerAndWriteServer();
                if (communication.data.ToString() == "?/delete user")
                {
                    communication.SendMessage("?/end");
                    check = 1;
                }
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
