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
        public async Task<bool> Run()
        {
            Entrance entrance = new Entrance(communication, @"D:\temp\User\user.json", writerGroups);
            var successConection = await entrance.ModeSelection();
            if (!successConection)
            {
                return false;
            }
            await writerGroups.Run(6);
            ConnectorToChat connectorToChat = new ConnectorToChat(communication, writerGroups);
            await connectorToChat.SelectChat();
            Console.ReadKey(true);
            return successConection;
        }
    }
}
