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
    class Entrance
    {
        public Entrance(Communication communication, string filePath, WriterGroups writerGroups)
        {
            this.communication = communication;
            this.FilePath = filePath;

            this.writerGroups = writerGroups;
        }
        FileMaster fileMaster = new FileMaster();
        Communication communication;
        private string FilePath { get; }
        private WriterGroups writerGroups;
        private bool LoginRegisteredAccount()
        {
            SendMessage("using");
            Console.WriteLine("Sign in with your previously entered nickname?" +
                " If yes, click Enter");
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                return SignInWithSavedAccount();
            }
            else
            {
                return SignInWithoutSavedAccount();
            }
        }
        private bool SignInWithoutSavedAccount()
        {
            var userData = EnterNicknameAndPassword();
            if (userData.Length == 0)
            {
                return false; 
            }
            else
            {
                AddAccountData(userData);
                return true;
            }
        }
        private bool SignInWithSavedAccount()
        {
            var finded = FindAccountAndEnter();
            if (finded)
            {
                return true;
            }
            else
            {
                return SignInWithoutSavedAccount();
            }
        }
        private void DeleteAccountData(string[] userData)
        {
            Console.WriteLine("If you want to delete your nickname and password from the device, click Enter");
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                userNicknamesAndPasswords = fileMaster.ReadDataToUser(FilePath);
                if (userNicknamesAndPasswords.Count() != 0)
                {
                    Console.WriteLine(userNicknamesAndPasswords.Count());
                    userNicknamesAndPasswords = userNicknamesAndPasswords
                        .Where(acc => acc.Nickname != userData[0] || acc.Password != userData[1])
                        .ToList();
                    fileMaster.WriteData(FilePath, userNicknamesAndPasswords);
                    Console.WriteLine("Your nickname and password were deleted");
                }
            }
        }
        private void AddAccountData(string[] userData)
        {
            Console.WriteLine("If you want to save your nickname and password to the device, click Enter");
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                userNicknamesAndPasswords = fileMaster.ReadDataToUser(FilePath);
                UserNicknameAndPassword userNicknameAndPassword = new UserNicknameAndPassword(userData[0], userData[1]);
                if (userNicknamesAndPasswords.Count() != 0)
                {
                    foreach (var oneUserNicknameAndPassword in userNicknamesAndPasswords)
                    {
                        if (oneUserNicknameAndPassword.Nickname == userNicknameAndPassword.Nickname && 
                            oneUserNicknameAndPassword.Password == userNicknameAndPassword.Password)
                        {
                            Console.WriteLine("You have this nickname and password");
                            return;
                        }
                        else if (oneUserNicknameAndPassword.Nickname == userNicknameAndPassword.Nickname &&
                            oneUserNicknameAndPassword.Password != userNicknameAndPassword.Password)
                        {
                            Console.WriteLine("You have this nickname but have another password,\n\r" +
                                "If you want to change data, click 'Enter'");
                            var chackRewrite = Console.ReadKey(true);
                            if (chackRewrite.Key == ConsoleKey.Enter)
                            {
                                RewriteDate();
                            }
                            return;
                        }
                    }
                }
                void RewriteDate()
                {
                    userNicknamesAndPasswords = userNicknamesAndPasswords
                        .Where(user => user.Nickname != userNicknameAndPassword.Nickname)
                        .ToList();
                    userNicknamesAndPasswords.Add(userNicknameAndPassword);
                    fileMaster.WriteData(FilePath, userNicknamesAndPasswords);
                    Console.WriteLine("Password changed");
                }
                fileMaster.AddData(userNicknameAndPassword, FilePath);
                Console.WriteLine("Saving is successful");
            }
        }
        List<UserNicknameAndPassword> userNicknamesAndPasswords;
        private bool FindAccountAndEnter()
        {
            userNicknamesAndPasswords = fileMaster.ReadDataToUser(FilePath);
            if (userNicknamesAndPasswords != null)
            {
                foreach (var userNicknameAndPassword in userNicknamesAndPasswords)
                {
                    Console.WriteLine(userNicknameAndPassword.Nickname);
                }
                Console.WriteLine("Enter need nickname");
                var numberOfAttempts = 5;
                for (int i = 0; i < numberOfAttempts; i++)
                {
                    var line = Console.ReadLine();
                    foreach (var userNicknameAndPassword in userNicknamesAndPasswords)
                    {
                        if (line == userNicknameAndPassword.Nickname)
                        {
                            return EnterWithSavedAccount(userNicknameAndPassword);
                        }
                    }
                    Console.WriteLine($"Don`t have this nickname, number of attempts left: {numberOfAttempts - i}");
                }
            }
            else
            {
                Console.WriteLine("Don`t have saved nickname, enter new");
            }
            return false;
        }
        private bool EnterWithSavedAccount(UserNicknameAndPassword userNicknameAndPassword)
        {
            AnswerServer();
            if (communication.data.ToString() == "Enter a nickname")
            {
                SendMessage(userNicknameAndPassword.Nickname);
                AnswerServer();
                if (communication.data.ToString() == "Enter password bigger than 7 symbols")
                {
                    SendMessage(userNicknameAndPassword.Password);
                    AnswerAndWriteServer();
                    if (communication.data.ToString() == "You enter to messenger")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private string[] EnterNicknameAndPassword()
        {
            while (true)
            {
                var nick = EnterNickname();
                if (nick.Length > 0)
                {
                    var password = EnterPassword();
                    if (password.Length > 7)
                    {
                        return new string[] { nick, password };
                    }
                }
                var key = Console.ReadKey();
                if (key.Key != ConsoleKey.Enter)
                {
                    SendMessage("No");
                    return new string[0];
                }
                SendMessage("Enter");
            }
        }
        private string EnterNickname()
        {
            AnswerAndWriteServer();
            while (true)
            {
                var nickname = Console.ReadLine();
                if (nickname.Length > 0)
                {
                    SendMessage(nickname);
                    AnswerAndWriteServer();
                    var s = communication.data.ToString().Substring(communication.data.ToString().Length - 5);
                    if (communication.data.ToString() == "Enter password bigger than 7 symbols")
                    {
                        return nickname;
                    }
                    else if (s == "Enter")
                    {
                        return "";
                    }
                }
                else
                {
                    Console.WriteLine("Nickname length < 1");
                }
            }
        }
        private string EnterPassword()
        {
            var i = 0;
            StringBuilder password = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    i++;
                    SendMessage(password.ToString());
                    AnswerServer();
                    if (communication.data.ToString() == "You enter to messenger")
                    {
                        Console.WriteLine();
                        return password.ToString();
                    }
                    else if (communication.data.ToString() == "This nickname is currently in use, bye" ||
                        communication.data.ToString().Substring(communication.data.Length - 5) == "Enter")
                    {
                        Console.WriteLine($"\n\r{communication.data}");
                        return "";
                    }
                    password = new StringBuilder();
                    Console.WriteLine($"\n{communication.data}");
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Remove(password.Length - 1, 1);
                    }
                }
                else
                {
                    Console.Write("*");
                    password.Append(key.KeyChar);
                }
            }
        }
        private bool LoginNewAccount()
        {
            SendMessage("new");
            return SignInWithoutSavedAccount();
        }
        private void ExitFromServer()
        {

        }
        public bool ModeSelection()
        {
            AnswerAndWriteServer();
            var successConnect = false;
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    successConnect = LoginRegisteredAccount();
                    break;
                case ConsoleKey.Tab:
                    successConnect = LoginNewAccount();
                    break;
                case ConsoleKey.Escape:
                    successConnect = EscapeServer();
                    break;
                case ConsoleKey.Delete:
                    successConnect = DeleteAccount();
                    break;
                default:
                    SendMessage("default");
                    successConnect = ModeSelection();
                    break;
            }
            return successConnect;
        }
        private bool EscapeServer()
        {
            SendMessage("escape");
            AnswerAndWriteServer();
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                SendMessage("Yes");
                AnswerAndWriteServer();
                return false;
            }
            SendMessage("No");
            AnswerAndWriteServer();
            SendMessage("Ok");
            return ModeSelection();
        }
        private bool DeleteAccount()
        {
            SendMessage("delete");
            var userData = EnterNicknameAndPassword();
            if (userData.Length == 0)
            {
                return false;
            }
            AnswerAndWriteServer();
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                SendMessage("Yes");
            }
            else
            {
                SendMessage("No");
                return false;
            }
            AnswerAndWriteServer();
            DeleteAccountData(userData);
            return false;
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
