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
            await communication.ListenServerWrite();
            while (!EndUsing)
            {
                var myMessage = Console.ReadLine();
                var serverMessage = await communication.SendMessageListenServerWrite(myMessage);
                await SelectMode(myMessage, serverMessage);
            }
        }
        private async Task SelectMode(string myMessage, string serverMessage)
        {
            if (serverMessage == "Enter name of chat" || serverMessage == "Enter user name")
            {
                await FindGroup();
                await OpenChat();
            }
            else if (serverMessage == $"If you want to join a group write: join{Environment.NewLine}" +
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
                await communication.ListenServerWrite();
            }
        }
        private async Task<bool> AcceptTheInvitation()
        {
            while (true)
            {
                var mode = Console.ReadLine();
                await communication.SendMessage(mode);
                if (mode == "look")
                {
                    await writerGroups.Run(1);
                }
                else if (mode == "join")
                {
                    await communication.ListenServerWrite();
                    while (true)
                    {
                        if (await communication.SendMessageListenServerWrite(Console.ReadLine()) == $"You have joined to the group{Environment.NewLine}" +
                            "If you want to open chats, write: 'open'")
                        {
                            if (await communication.SendMessageListenServerWrite(Console.ReadLine()) == "You enter to the group")
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                }
                else
                {
                    await communication.ListenServerWrite();
                }
            }
        }
        private async Task FindGroup()
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (await communication.SendMessageListenServerWrite(line) == "You connect to chat")
                {
                    return;
                }
            }
        }
        private async Task OpenChat()
        {
            Chat chat = new Chat(communication);
            await chat.Run();
            chat.autoResetConnectAgain.WaitOne();

            await communication.ListenServerWrite();
            if (await communication.SendMessageListenServerWrite(Console.ReadLine()) == "You left the messanger")
            {
                EndUsing = true;
                return;
            }
            await writerGroups.Run(6);
            await communication.ListenServerWrite();
        }
        private async Task<bool> CreateNewGroup()
        {
            while (true)
            {
                await communication.ListenServerWrite();
                await WriteToServer("Enter a group name");
                await WriteToServer($"Who do you want to invite to your group?{Environment.NewLine}" +
                            $"If you want to check people, write ?/yes{Environment.NewLine}" +
                            $"If you don`t want to add people, write ?/no{Environment.NewLine}");
                await WriteToServer($"You created a group, thanks.{Environment.NewLine}" +
                    "If you want to open it - write ok, else - write else");
                var line = Console.ReadLine();
                await communication.SendMessage(line);
                if (line == "ok")
                {
                    return true;
                }
                return false;
            }
        }
        private async Task WriteToServer(string finalMesage)
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line.Length > 0)
                {
                    var message = await communication.SendMessageListenServerWrite(line);
                    if (message == finalMesage)
                    {
                        break;
                    }
                    if (message == "People:")
                    {
                        await writerGroups.WriteGroup(Console.CursorLeft, Console.CursorTop);
                    }
                }
            }
        }
    }
}
