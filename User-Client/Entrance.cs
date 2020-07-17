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
        //public bool Run()
        //{
        //    ConnectorToChat connectorToChat = new ConnectorToChat(communication, @"D:\temp\User\user.json");
        //    var successConection = connectorToChat.ModeSelection();
        //    //if (!successConection)
        //    //{
        //    //    SendMessage("?");
        //    //}
        //    writerGroups.Run(6);
        //    SelectChat();
        //    Console.ReadKey();
        //    return successConection;
        //}
        //private void WritePublickChats()
        //{
        //    Console.SetCursorPosition(MaxWidth/2, Console.CursorTop);
        //    WriteSaveChats("Public groups:");
        //    Console.SetCursorPosition(0, Console.CursorTop);
        //}
        //private void WriteSaveChats(string groups)
        //{
        //    AnswerAndWriteServer();
        //    SendMessage("Ok");
        //    AnswerServer();
        //    var countChats = Convert.ToInt32(data);
        //    if (countChats == 0)
        //    {
        //        SendMessage("Ok");
        //        Console.WriteLine("(don`t have)");
        //    }
        //    else
        //    {
        //        SendMessage("Send goups");
        //        for (int i = 0; i < countChats; i++)
        //        {
        //            AnswerServer();
        //            if (data.Length > countChats / 2)
        //            {
        //                WriteBigNameChat(MaxWidth);
        //                continue;
        //            }
        //            Console.WriteLine($"{data}\n\t"); ////////////////////// data - stringbuilder
        //            SendMessage("+");
        //        }
        //    }
        //}
        private bool LoginTheNicknameUsed()
        {
            SendMessage("using");
            Console.WriteLine("Sign in with your previously entered nickname?" +
                " If yes, click Enter");
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                return SignInWithPreviouslyEnteredNickname();
            }
            else
            {
                return SignInWithoutPreviouslyEnteredNickname();
            }
        }
        private bool SignInWithoutPreviouslyEnteredNickname()
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
        private bool SignInWithPreviouslyEnteredNickname()
        {
            var finded = FindNeedNickAndEntrance();
            if (finded)
            {
                return true;
            }
            else
            {
                return SignInWithoutPreviouslyEnteredNickname();
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
                //var buffer = 256;
                //StringBuilder userJson = new StringBuilder();
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
                //using (FileStream fileStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                //{
                //    //UserNicknameAndPassword userNicknameAndPassword = new UserNicknameAndPassword(userData[0], userData[1]);
                //    var arrayBytesRead = new byte[buffer];
                //    while (true)
                //    {
                //        var readedRealBytes = fileStream.Read(arrayBytesRead, 0, buffer);
                //        var stringArrayBytes = Encoding.Default.GetString(arrayBytesRead, 0, readedRealBytes);
                //        userJson.Append(stringArrayBytes);
                //        if (readedRealBytes < buffer)
                //        {
                //            break;
                //        }
                //    }
                //    var userNicknamesAndPasswords = JsonConvert.DeserializeObject<List<UserNicknameAndPassword>>(userJson.ToString());
                //    string userDataJson;
                //    if (userNicknamesAndPasswords == null)
                //    {
                //        List<UserNicknameAndPassword> userNicknameAndPasswordList = new List<UserNicknameAndPassword>();
                //        userNicknameAndPasswordList.Add(userNicknameAndPassword);
                //        userDataJson = JsonConvert.SerializeObject(userNicknameAndPasswordList);
                //    }
                //    else
                //    {
                //        userNicknamesAndPasswords.Add(userNicknameAndPassword);
                //        userDataJson = JsonConvert.SerializeObject(userNicknamesAndPasswords);
                //    }
                //    var arrayBytesWrite = Encoding.Default.GetBytes(userDataJson);
                //    fileStream.Seek(0, SeekOrigin.Begin);
                //    fileStream.Write(arrayBytesWrite, 0, arrayBytesWrite.Length);
                //    Console.WriteLine("Saving is successful");
                //}
            }
        }
        List<UserNicknameAndPassword> userNicknamesAndPasswords;
        private bool FindNeedNickAndEntrance()
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
                            return EntrenceWithUsedNick(userNicknameAndPassword);
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
        //private IEnumerable<UserNicknameAndPassword> ReadUserData()
        //{
        //    var buffer = 256;
        //    StringBuilder userJson = new StringBuilder();
        //    using (FileStream fileStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        //    {
        //        var arrayBytesRead = new byte[buffer];
        //        while (true)
        //        {
        //            var readedRealBytes = fileStream.Read(arrayBytesRead, 0, buffer);
        //            var stringArrayBytes = Encoding.Default.GetString(arrayBytesRead, 0, readedRealBytes);
        //            userJson.Append(stringArrayBytes);
        //            if (readedRealBytes < buffer)
        //            {
        //                break;
        //            }
        //        }
        //        return JsonConvert.DeserializeObject<IEnumerable<UserNicknameAndPassword>>(userJson.ToString());
        //    }
        //}
        private bool EntrenceWithUsedNick(UserNicknameAndPassword userNicknameAndPassword)
        {
            AnswerAndWriteServer();
            //AnswerServer();
            if (communication.data.ToString() == "Enter a nickname")
            {
                SendMessage(userNicknameAndPassword.Nickname);
                AnswerAndWriteServer();
                //AnswerServer();
                if (communication.data.ToString() == "Enter password bigger than 7 symbols")
                {
                    SendMessage(userNicknameAndPassword.Password);
                    AnswerAndWriteServer();
                    //AnswerServer();
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
        //private string EnterPassword()
        //{
        //    for (int i = 0; i <= 5; i++)
        //    {
        //        StringBuilder password = new StringBuilder();
        //        while (true)
        //        {
        //            var key = Console.ReadKey(true);
        //            if (key.Key == ConsoleKey.Enter)
        //            {
        //                if (password.Length > 7)
        //                {
        //                    SendMessage(password.ToString());
        //                    break;
        //                }
        //                else
        //                {
        //                    password = new StringBuilder();
        //                    Console.WriteLine("Password length < 7, enter another password");
        //                }
        //            }
        //            else if (key.Key == ConsoleKey.Backspace)
        //            {
        //                if (password.Length > 0)
        //                {
        //                    password.Remove(password.Length - 1, 1);
        //                }
        //            }
        //            else
        //            {
        //                Console.Write("*");
        //                password.Append(key.KeyChar);
        //            }
        //        }
        //        Console.WriteLine();
        //        AnswerAndWriteServer();
        //        if (data.ToString() == "LastCheck")
        //        {
        //            return password.ToString();
        //        }
        //    }
        //    return "";
        //}
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
            //AnswerAndWriteServer();
            //if (communication.data.ToString() == "LastCheck")
            return "";
        }
        private bool LoginTheNicknameNotUsed()
        {
            SendMessage("new");
            return SignInWithoutPreviouslyEnteredNickname();
        }
        private void ExitFromServer()
        {

        }
        private void DeleteNicknameInServer()
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
                    successConnect = LoginTheNicknameUsed();
                    break;
                case ConsoleKey.Tab:
                    successConnect = LoginTheNicknameNotUsed();
                    break;
                case ConsoleKey.Escape:
                    successConnect = EscapeTheServer();
                    break;
                case ConsoleKey.Delete:
                    successConnect = DeleteAccount();
                    break;
                default:
                    SendMessage("default");
                    ModeSelection();
                    break;
            }
            return successConnect;
        }
        private bool EscapeTheServer()
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
            //if (communication.data.ToString() == "Don`t have this nickname" || communication.data.ToString() == "Index was deleter")
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
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
