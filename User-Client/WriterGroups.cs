using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class WriterGroups
    {
        public WriterGroups(Communication communication)
        {
            this.communication = communication;
        }
        Communication communication;
        private int MaxWidth { get { return Console.BufferWidth; } }
        public void Run(int count)
        {
            WriteGroups(count);
        }
        private void WriteGroups(int count)
        {
            var cursorTop = Console.CursorTop;
            var halfOfMaxWidth = MaxWidth / 2;
            int lustTopPosition = 0;
            for (int i = 0; i < count; i++)
            {
                if (i > 1)
                {
                    Console.SetCursorPosition(halfOfMaxWidth, cursorTop);
                    AnswerAndWriteServer();
                    cursorTop = Console.CursorTop;
                    SendMessage("Ok");
                    WriteGroup(halfOfMaxWidth, cursorTop);
                    cursorTop = Console.CursorTop;
                    continue;
                }
                AnswerAndWriteServer();
                SendMessage("Ok");
                WriteGroup(Console.CursorLeft, Console.CursorTop);
                lustTopPosition = Console.CursorTop;
            }
            if (cursorTop > lustTopPosition)
            {
                Console.SetCursorPosition(0, cursorTop + 1);
            }
            else
            {
                Console.SetCursorPosition(0, lustTopPosition + 1);
            }
        }
        private void WriteInSomePosition(string message, int weigh, int top)
        {
            Console.SetCursorPosition(weigh, top);
            Console.WriteLine(message);
        }
        public void WriteGroup(int weigh, int top)
        {
            AnswerServer();
            var countChats = Convert.ToInt32(communication.data.ToString());
            if (countChats == 0)
            {
                SendMessage("Ok");
                WriteInSomePosition("(don`t have)", weigh, top);
            }
            else
            {
                SendMessage("Send goups");
                for (int i = 0; i < countChats; i++)
                {
                    AnswerServer();
                    if (communication.data.Length > MaxWidth / 2)
                    {
                        top = WriteBigNameChat(weigh, top);
                    }
                    else
                    {
                        WriteInSomePosition($"{communication.data}", weigh, top);
                    }
                    SendMessage("+");
                    top++;
                }
            }
        }
        private int WriteBigNameChat(int weidth, int top)
        {
            var firstWeidth = weidth;
            var halfOfMaxWidth = MaxWidth / 2 - 1;
            for (int i = 0; i < communication.data.Length; i++)
            {
                if (i % halfOfMaxWidth == 0 && i != 0)
                {
                    top++;
                    Console.SetCursorPosition(firstWeidth, top);
                    weidth = firstWeidth + 1;
                    Console.WriteLine(communication.data[i]);
                    continue;
                }
                Console.SetCursorPosition(weidth, top);
                Console.Write(communication.data[i]);
                weidth++;
            }
            return top;
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
