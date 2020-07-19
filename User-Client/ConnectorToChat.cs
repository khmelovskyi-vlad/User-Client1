using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class ConnectorToChat
    {
        public ConnectorToChat(Communication communication, WriterGroups writerGroups)
        {
            this.communication = communication;
            this.writerGroups = writerGroups;
        }
        private WriterGroups writerGroups;
        Communication communication;
        public void SelectChat()
        {
            AnswerAndWriteServer();
            while (!EndUsing)
            {
                var line = Console.ReadLine();
                if (line.Length != 0)
                {
                    SendMessage(line);
                    AnswerAndWriteServer();
                    ModeSelection(line);
                    //if (EndUsing)
                    //{
                    //    return;
                    //}
                }
            }
        }
        private bool EndUsing = false;
        private void ModeSelection(string message)
        {
            var serverMessage = communication.data.ToString();
            if (serverMessage == "Enter name of chat" || serverMessage == "Enter user name")
            {
                FindGroup();
                OpenChat();
            }
            else if (serverMessage == "If you want to join a group write: join\n\r" +
                "if you want to look at the invitations, write: look")
            {
                if (AcceptTheInvitation())
                {
                    OpenChat();
                }
            }
            else if (serverMessage == "You exit messanger")
            {
                EndUsing = true;
            }
            else
            {
                if (message.Length > 3 && message[0] == '?' && message[1] == '/')
                {
                    var first4 = message.Substring(0, 4);
                    SendMessage("I am waiting");
                    if (message == "?/ng")
                    {
                        var needOpenChat = CreateNewGroup();
                        if (needOpenChat)
                        {
                            OpenChat();
                        }
                        else
                        {
                            EndUsing = true;
                        }
                    }
                    else if (first4 == "?/gg")
                    {
                        writerGroups.Run(4);
                    }
                    else if (first4 == "?/cc")
                    {
                        writerGroups.Run(6);
                    }
                    else if(first4 == "?/pp" || first4 == "?/ch" || first4 == "?/sg" || first4 == "?/ug" || first4 == "?/ii" || first4 == "?/pg")
                    {
                        writerGroups.Run(1);
                    }
                    //if (communication.data.ToString() == "You connect to chat")
                    //{
                    //    return;
                    //}
                    AnswerAndWriteServer();
                }
            }
        }
        private bool AcceptTheInvitation()
        {
            while (true)
            {
                var mode = Console.ReadLine();
                if (mode.Length > 0)
                {
                    SendMessage(mode);
                    if (mode == "look")
                    {
                        writerGroups.Run(1);
                    }
                    else if (mode == "join")
                    {
                        AnswerAndWriteServer();
                        while (true)
                        {
                            var groupName = Console.ReadLine();
                            if (groupName.Length > 0)
                            {
                                SendMessage(groupName);
                                AnswerAndWriteServer();
                                if (communication.data.ToString() == "You have joined to the group\n\r" +
                                    "If you want to open chats, write: 'open'")
                                {
                                    var enteranceToGroup = Console.ReadLine();
                                    if (enteranceToGroup.Length > 0)
                                    {
                                        SendMessage(enteranceToGroup);
                                        AnswerAndWriteServer();
                                        if (communication.data.ToString() == "You enter to the group")
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
                        }
                    }
                    else
                    {
                        AnswerAndWriteServer();
                    }
                }
            }
        }
        private void FindGroup()
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Length > 0)
                {
                    SendMessage(line);
                    AnswerAndWriteServer();
                    if (communication.data.ToString() == "You connect to chat")
                    {
                        return;
                    }
                }
            }
        }
        private void OpenChat()
        {
            Chat chat = new Chat(communication);
            chat.Run();
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Length > 0)
                {
                    communication.SendMessage(line);
                    communication.AnswerAndWriteServer();
                    if (communication.data.ToString() == "You left the messanger")
                    {
                        EndUsing = true;
                        return;
                    }
                    writerGroups.Run(6);
                    AnswerAndWriteServer();
                    return;
                }
            }
            //Console.WriteLine("ok, good");
            //Console.ReadKey();
        }
        private bool CreateNewGroup()
        {
            while (true)
            {
                AnswerAndWriteServer();
                WriteToServer("Enter a group name");
                WriteToServer("Who do you want to invite to your group?\n\r" +
                            "If you want to check people, write ?/yes\n\r" +
                            "If you don`t want to add people, write ?/no\n\r");
                WriteToServer("You create group, thanks.\n\r" +
                    "If you want to open it, write ok, else - press else");
                while (true)
                {
                    var lineTwo = Console.ReadLine();
                    if (lineTwo.Length > 0)
                    {
                        SendMessage(lineTwo);
                        if (lineTwo == "ok")
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
        }
        private void WriteToServer(string finalMesage)
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Length > 0)
                {
                    SendMessage(line);
                    AnswerAndWriteServer();
                    if (communication.data.ToString() == finalMesage)
                    {
                        break;
                    }
                    if (communication.data.ToString() == "People:")
                    {
                        SendMessage("Ok");
                        writerGroups.WriteGroup(Console.CursorLeft, Console.CursorTop);
                    }
                }
            }
        }
        private void AnswerAndWriteServer()
        {
            AnswerServer();
            Console.WriteLine(communication.data);
        }
        private void AnswerServer()
        {
            communication.AnswerServer();
        }
        private void SendMessage(string message)
        {
            communication.SendMessage(message);
        }
    }
}
