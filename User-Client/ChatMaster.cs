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
        //private string FilePath { get { return @"D:\temp\User\user.json"; } }
        private WriterGroups writerGroups;
        public bool Run()
        {
            Entrance entrance = new Entrance(communication, @"D:\temp\User\user.json", writerGroups);
            var successConection = entrance.ModeSelection();
            //if (!successConection)
            //{
            //    return false;
            //}
            writerGroups.Run(6);
            ConnectorToChat connectorToChat = new ConnectorToChat(communication, writerGroups);
            connectorToChat.SelectChat();
            Console.ReadKey(true);
            return successConection;
        }
        //public bool Run()
        //{
        //    Entrance entrance = new Entrance(communication, @"D:\temp\User\user.json", writerGroups);
        //    var successConection = entrance.ModeSelection();
        //    //if (!successConection)
        //    //{
        //    //    SendMessage("?");
        //    //}
        //    writerGroups.Run(6);
        //    Console.ReadKey();
        //    return successConection;
        //}
        //private void ModeSelection()
        //{
        //    while (true)
        //    {
        //        communication.AnswerAndWriteServer();
        //        if (communication.data.ToString() == "Choose group")
        //        {
        //            communication.SendMessage("Ok");
        //            ConnectorToChat connectorToChat = new ConnectorToChat(communication, writerGroups);
        //            connectorToChat.SelectChat();
        //            break;
        //        }
        //        else if (communication.data.ToString() == "Select the group you want to leave")
        //        {
        //            communication.SendMessage("Ok");

        //            break;
        //        }
        //        var line = Console.ReadLine();
        //        if (line.Length >= 0)
        //        {
        //            communication.SendMessage(line);
        //        }
        //    }
        //}
    }
}
