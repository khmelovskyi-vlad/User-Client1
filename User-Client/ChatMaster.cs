using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class ChatMaster
    {
        public ChatMaster(Communication communication)
        {
            this.communication = communication;
            writerGroups = new WriterGroups(communication);
        }
        Communication communication;
        private WriterGroups writerGroups;
        public bool Run()
        {
            Entrance entrance = new Entrance(communication, @"D:\temp\User\user.json", writerGroups);
            var successConection = entrance.ModeSelection();
            if (!successConection)
            {
                return false;
            }
            writerGroups.Run(6);
            ConnectorToChat connectorToChat = new ConnectorToChat(communication, writerGroups);
            connectorToChat.SelectChat();
            Console.ReadKey(true);
            return successConection;
        }
    }
}
