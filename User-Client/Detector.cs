using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class Detector
    {
        public Detector()
        {
        }
        private StringBuilder allText = new StringBuilder();
        public void Run()
        {
            var connector = new Connector();
            try
            {
                while (true)
                {
                    var userInformation = EnterData();
                    connector.Run(userInformation);
                    Console.WriteLine("Bed input, try again");
                    allText.Remove(0, allText.Length - 1);
                    allText.Append("Bed input, try again\n\r");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }

        }
        private string[] EnterData()
        {
            var ip = "192.168.0.101";
            //var ip = "192.168.1.240";
            var port = "1234";
            var nickname = "Vlad";
            var password = "123";
            //var ip = EnterSomeData("Enter your IP");
            //var port = EnterSomeData("Enter need port");
            //var nickname = EnterSomeData("Enter your nickname");
            //var password = EnterPassword();
            return new string[] { ip, port, nickname, password };
        }
        private string EnterSomeData(string message)
        {
            Console.WriteLine(message);
            var data = Console.ReadLine();
            allText.Append($"{message}\n\r{data}\n\r");
            return data;
        }
        private string EnterPassword()
        {
            StringBuilder password = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    allText.Append("\n\r");
                    return password.ToString();
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    allText.Remove(allText.Length - 1, 1);
                    Console.Clear();
                    Console.Write(allText);
                    password.Remove(password.Length - 1, 1);
                }
                else
                {
                    Console.Write("*");
                    allText.Append(key.KeyChar);
                    password.Append(key.KeyChar);
                }
            }
        }
    }
}
