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
        private Communication communication;
        private bool EndUsing = false;
        public async Task SelectChat()
        {
            await communication.AnswerAndWriteServer();
            while (!EndUsing)
            {
                var myMessage = Console.ReadLine();
                if (myMessage.Length != 0)
                {
                    await communication.SendMessage(myMessage);
                    await communication.AnswerAndWriteServer();
                    await SelectMode(myMessage);
                }
            }
        }
        private async Task SelectMode(string myMessage)
        {
            var serverMessage = communication.data.ToString();
            if (serverMessage == "Enter name of chat" || serverMessage == "Enter user name")
            {
                await FindGroup();
                await OpenChat();
            }
            else if (serverMessage == "If you want to join a group write: join\n\r" +
                "if you want to look at the invitations, write: look")
            {
                if (await AcceptTheInvitation())
                {
                    await OpenChat();
                }
                else
                {
                    EndUsing = true;
                }
            }
            else if (serverMessage == "You exit messanger")
            {
                EndUsing = true;
            }
            else
            {
                await SelectModeAccordingMyMessage(myMessage);
            }
        }
        private async Task SelectModeAccordingMyMessage(string myMessage)
        {
            if (myMessage.Length > 3 && myMessage[0] == '?' && myMessage[1] == '/')
            {
                var first4 = myMessage.Substring(0, 4);
                await communication.SendMessage("I am waiting");
                if (myMessage == "?/ng")
                {
                    var needOpenChat = await CreateNewGroup();
                    if (needOpenChat)
                    {
                        await OpenChat();
                        return;
                    }
                    else
                    {
                        EndUsing = true;
                    }
                }
                else if (first4 == "?/gg")
                {
                    await writerGroups.Run(4);
                }
                else if (first4 == "?/cc")
                {
                    await writerGroups.Run(6);
                }
                else if (first4 == "?/pp" || first4 == "?/ch" || first4 == "?/sg" || first4 == "?/ug" || first4 == "?/ii" || first4 == "?/pg")
                {
                    await writerGroups.Run(1);
                }
                await communication.AnswerAndWriteServer();
            }
        }
        private async Task<bool> AcceptTheInvitation()
        {
            while (true)
            {
                var mode = Console.ReadLine();
                if (mode.Length > 0)
                {
                    await communication.SendMessage(mode);
                    if (mode == "look")
                    {
                        await writerGroups.Run(1);
                    }
                    else if (mode == "join")
                    {
                        await communication.AnswerAndWriteServer();
                        while (true)
                        {
                            var groupName = Console.ReadLine();
                            if (groupName.Length > 0)
                            {
                                await communication.SendMessage(groupName);
                                await communication.AnswerAndWriteServer();
                                if (communication.data.ToString() == "You have joined to the group\n\r" +
                                    "If you want to open chats, write: 'open'")
                                {
                                    var enteranceToGroup = Console.ReadLine();
                                    if (enteranceToGroup.Length > 0)
                                    {
                                        await communication.SendMessage(enteranceToGroup);
                                        await communication.AnswerAndWriteServer();
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
                        await communication.AnswerAndWriteServer();
                    }
                }
            }
        }
        private async Task FindGroup()
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Length > 0)
                {
                    await communication.SendMessage(line);
                    await communication.AnswerAndWriteServer();
                    if (communication.data.ToString() == "You connect to chat")
                    {
                        return;
                    }
                }
            }
        }
        private async Task OpenChat()
        {
            Chat chat = new Chat(communication);
            await chat.Run();
            chat.autoResetConnectAgain.WaitOne();
            await communication.SendMessage("I left the chat");
            await communication.AnswerAndWriteServer();
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Length > 0)
                {
                    await communication.SendMessage(line);
                    await communication.AnswerAndWriteServer();
                    await communication.SendMessage("Okey");
                    if (communication.data.ToString() == "You left the messanger")
                    {
                        EndUsing = true;
                        return;
                    }
                    await writerGroups.Run(6);
                    await communication.AnswerAndWriteServer();
                    return;
                }
            }
        }
        private async Task<bool> CreateNewGroup()
        {
            while (true)
            {
                await communication.AnswerAndWriteServer();
                await WriteToServer("Enter a group name");
                await WriteToServer("Who do you want to invite to your group?\n\r" +
                            "If you want to check people, write ?/yes\n\r" +
                            "If you don`t want to add people, write ?/no\n\r");
                await WriteToServer("You create group, thanks.\n\r" +
                    "If you want to open it, write ok, else - press else");
                while (true)
                {
                    var lineTwo = Console.ReadLine();
                    if (lineTwo.Length > 0)
                    {
                        await communication.SendMessage(lineTwo);
                        if (lineTwo == "ok")
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
        }
        private async Task WriteToServer(string finalMesage)
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Length > 0)
                {
                    await communication.SendMessage(line);
                    await communication.AnswerAndWriteServer();
                    if (communication.data.ToString() == finalMesage)
                    {
                        break;
                    }
                    if (communication.data.ToString() == "People:")
                    {
                        await communication.SendMessage("Ok");
                        await writerGroups.WriteGroup(Console.CursorLeft, Console.CursorTop);
                    }
                }
            }
        }
    }
}
