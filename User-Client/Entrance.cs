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
        private async Task<bool> LoginRegisteredAccount()
        {
            SendMessage("using");
            Console.WriteLine("Sign in with your previously entered nickname?" +
                " If yes, click Enter");
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                return await SignInWithSavedAccount();
            }
            else
            {
                return await SignInWithoutSavedAccount();
            }
        }
        private async Task<bool> SignInWithoutSavedAccount()
        {
            var userData = EnterNicknameAndPassword();
            if (userData.Length == 0)
            {
                return false; 
            }
            else
            {
                await AddAccountData(userData);
                return true;
            }
        }
        private async Task<bool> SignInWithSavedAccount()
        {
            var finded = await FindAccountAndEnter();
            if (finded)
            {
                return true;
            }
            else
            {
                return await SignInWithoutSavedAccount();
            }
        }
        private async Task DeleteAccountData(string[] userData)
        {
            Console.WriteLine("If you want to delete your nickname and password from the device, click Enter");
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                userNicknamesAndPasswords = await fileMaster.ReadData<UserNicknameAndPassword>(FilePath);
                if (userNicknamesAndPasswords.Count() != 0)
                {
                    Console.WriteLine(userNicknamesAndPasswords.Count());
                    userNicknamesAndPasswords = userNicknamesAndPasswords
                        .Where(acc => acc.Nickname != userData[0] || acc.Password != userData[1])
                        .ToList();
                    await fileMaster.WriteData(FilePath, userNicknamesAndPasswords);
                    Console.WriteLine("Your nickname and password were deleted");
                }
            }
        }
        private async Task AddAccountData(string[] userData)
        {
            Console.WriteLine("If you want to save your nickname and password to the device, click Enter");
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                userNicknamesAndPasswords = await fileMaster.ReadData<UserNicknameAndPassword>(FilePath);
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
                                await RewriteDate();
                            }
                            return;
                        }
                    }
                }
                async Task RewriteDate()
                {
                    userNicknamesAndPasswords = userNicknamesAndPasswords
                        .Where(user => user.Nickname != userNicknameAndPassword.Nickname)
                        .ToList();
                    userNicknamesAndPasswords.Add(userNicknameAndPassword);
                    await fileMaster.WriteData(FilePath, userNicknamesAndPasswords);
                    Console.WriteLine("Password changed");
                }
                await fileMaster.AddData(userNicknameAndPassword, FilePath);
                Console.WriteLine("Saving is successful");
            }
        }
        List<UserNicknameAndPassword> userNicknamesAndPasswords;
        private async Task<bool> FindAccountAndEnter()
        {
            userNicknamesAndPasswords = await fileMaster.ReadData<UserNicknameAndPassword>(FilePath);
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
        private async Task<bool> LoginNewAccount()
        {
            SendMessage("new");
            return await SignInWithoutSavedAccount();
        }
        private void ExitFromServer()
        {

        }
        public async Task<bool> ModeSelection()
        {
            AnswerAndWriteServer();
            var successConnect = false;
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    successConnect = await LoginRegisteredAccount();
                    break;
                case ConsoleKey.Tab:
                    successConnect = await LoginNewAccount();
                    break;
                case ConsoleKey.Escape:
                    successConnect = await EscapeServer();
                    break;
                case ConsoleKey.Delete:
                    successConnect = await DeleteAccount();
                    break;
                default:
                    SendMessage("default");
                    successConnect = await ModeSelection();
                    break;
            }
            return successConnect;
        }
        private async Task<bool> EscapeServer()
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
            return await ModeSelection();
        }
        private async Task<bool> DeleteAccount()
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
            await DeleteAccountData(userData);
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
