using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Client
{
    class FileManager
    {
        private IUserInteractor userInteractor;

        public FileManager(IUserInteractor userInteractor)
        {
            this.userInteractor = userInteractor;
        }

        private string adressName { get; set; }
        StringBuilder textBuilderEnd;
        StringBuilder allTextBuilder;
        StringBuilder textBuilderFirst;
        int topPosition;
        int leftPosition;
        string[] fileLines;
        private bool AdressHaving()
        {
            return File.Exists(adressName);
        }
        public void FileManage()
        {
            SelectDisk();
            while (true)
            {
                var mode = userInteractor.AskAction();
                switch (mode)
                {
                    case UserAction.SelectFolder:
                        SelectFolder();
                        break;
                    case UserAction.BackFolder:
                        BackFolder();
                        break;
                    case UserAction.Close:
                        return;
                }
            }
        }
        private void BackFolder()
        {
            try
            {
                adressName = adressName.Remove(adressName.Length - 1);
                adressName = Path.GetDirectoryName(adressName);
                ReadFolder();
            }
            catch (ArgumentNullException)
            {
                SelectDisk();
            }
        }
        private void SelectFolder()
        {
            Console.WriteLine("Select a folder");
            while (true)
            {
                var saveAdress = adressName;
                try
                {
                    var fileName = Console.ReadLine();
                    var adress = $"{fileName}\\";
                    adressName = $"{adressName}{adress}";
                    ReadFolder();
                    break;
                }
                catch (DirectoryNotFoundException ex)
                {
                    adressName = saveAdress;
                    Console.WriteLine($"Bed input {ex}, try again");
                }
                catch (IOException)
                {
                    adressName = adressName.Substring(0, adressName.Length - 1);
                    EditingFile();
                    break;
                }
            }
        }
        private void SelectDisk()
        {
            Console.Write("You have disks: ");
            var allDisks = Directory.GetLogicalDrives();
            foreach (var disk in allDisks)
            {
                Console.Write($"{disk}, ");
            }
            Console.WriteLine();
            Console.WriteLine("Select a disc");
            while (true)
            {
                Console.WriteLine("Write name your disk");
                var line = Console.ReadLine();
                adressName = $"{line}:\\";
                var flag = false;
                foreach (var disk in allDisks)
                {
                    if (disk == adressName)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    ReadFolder();
                    break;
                }
            }
        }
        private void ReadFolder()
        {
            var allDirectories = Directory.GetDirectories(adressName);
            var allFiles = Directory.GetFiles(adressName);
            //var allFiles = allDirectories.Concat(allDocument);
            userInteractor.ShowFolderForlders(allDirectories);
            userInteractor.ShowFolderFiles(allFiles);
        }
        private void EditingFile()
        {
            var undestand = UnderstandingQuestions();
            if (undestand.Key == ConsoleKey.Enter)
            {
                InitializationOfVariables();
                if (LeftOrTopPositionIsOrNotTooBig())
                {
                    return;
                }
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.LeftArrow)
                    {
                        LeftArrow();
                    }
                    else if (key.Key == ConsoleKey.RightArrow)
                    {
                        RightArrow();
                    }
                    else if (key.Key == ConsoleKey.UpArrow)
                    {
                        topPosition--;
                    }
                    else if (key.Key == ConsoleKey.DownArrow)
                    {
                        topPosition++;
                    }
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        Backspace();
                    }
                    else if (key.Key == ConsoleKey.Delete)
                    {
                        Delete();
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        Enter();
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    else
                    {
                        ElseClick(key);
                    }
                    if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow)
                    {
                        UpOrDownArrow();
                    }
                    Cursor();
                }
                SavingFile(fileLines);
            }
            BackFolder();
        }
        private bool LeftOrTopPositionIsOrNotTooBig()
        {
            try
            {
                Console.SetCursorPosition(leftPosition, topPosition);
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("This text is too big, can`t redact it");
                BackFolder();
                return true;
            }
        }
        private void Cursor()
        {
            try
            {
                Console.SetCursorPosition(leftPosition, topPosition);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.Clear();
                Console.WriteLine("Console window is too small, expand it and any key except Escape, or if you want exit the program click Escape");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    throw new OperationCanceledException();
                }
                else
                {
                    InitializationOfVariables();
                    Cursor();
                }
            }
        }
        private void InitializationOfVariables()
        {
            Console.Clear();
            fileLines = File.ReadAllLines(adressName);
            WriteLines(fileLines);
            topPosition = fileLines.Length - 1;
            textBuilderFirst = new StringBuilder(fileLines[topPosition]);
            textBuilderEnd = new StringBuilder();
            allTextBuilder = new StringBuilder();
            leftPosition = textBuilderFirst.Length;
        }
        private ConsoleKeyInfo UnderstandingQuestions()
        {
            Console.WriteLine("If you want to edit the text click Enter, else click another key");
            Console.WriteLine("If you press enter, only the text of your file will be displayed on the console, at the end of editing press Escape");
            var key = Console.ReadKey(true);
            return key;
        }
        private void SavingFile(string[] fileLine)
        {
            Console.Clear();
            Console.WriteLine("If you want to save this file, click Enter, else, enouther click");
            var key = Console.ReadKey(true);
            Console.Clear();
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine("File was saving");
                File.WriteAllLines(adressName, fileLine, Encoding.Default);
            }
        }
        private static void WriteLines(string[] fileStrings)
        {
            foreach (var fileString in fileStrings)
            {
                Console.WriteLine(fileString);
            }
        }
        private void LeftArrow()
        {
            if (leftPosition != 0)
            {
                textBuilderEnd.Insert(0, textBuilderFirst[textBuilderFirst.Length - 1]);
                textBuilderFirst.Remove(textBuilderFirst.Length - 1, 1);
                leftPosition--;
            }
            else
            {
                if (leftPosition == 0 && topPosition == 0)
                {
                    leftPosition = 0;
                    return;
                }
                topPosition--;
                textBuilderFirst = new StringBuilder(fileLines[topPosition]);
                leftPosition = textBuilderFirst.Length;
                textBuilderEnd.Clear();
            }
            return;
        }
        private void RightArrow()
        {
            var lengthFiletLine = fileLines[topPosition].Length;
            if (leftPosition != lengthFiletLine)
            {
                textBuilderFirst.Append(textBuilderEnd[0]);
                textBuilderEnd.Remove(0, 1);
                leftPosition++;
            }
            else
            {
                if (leftPosition == lengthFiletLine && topPosition == fileLines.Length - 1)
                {
                    leftPosition = textBuilderFirst.Length;
                    return;
                }
                topPosition++;
                textBuilderFirst.Clear();
                leftPosition = 0;
                textBuilderEnd = new StringBuilder(fileLines[topPosition]);
            }
            return;
        }
        private void UpOrDownArrow()
        {
            if (topPosition == fileLines.Length)
            {
                topPosition = fileLines.Length - 1;
                return;
            }
            if (topPosition < 0)
            {
                topPosition = 0;
                return;
            }
            textBuilderFirst = new StringBuilder(fileLines[topPosition]);
            var countEndBuilder = textBuilderFirst.Length - leftPosition;
            textBuilderEnd.Clear();
            if (countEndBuilder >= 0)
            {
                textBuilderEnd.Append(Convert.ToString(textBuilderFirst), leftPosition, countEndBuilder);
                textBuilderFirst.Remove(leftPosition, countEndBuilder);
            }
            else
            {
                leftPosition = textBuilderFirst.Length;
            }
            return;
        }
        private void Backspace()
        {
            allTextBuilder.Clear();
            Console.Clear();
            if (textBuilderFirst.Length != 0)
            {
                textBuilderFirst.Remove(textBuilderFirst.Length - 1, 1);
            }
            else
            {
                topPosition--;
                if (topPosition >= 0)
                {
                    textBuilderFirst = new StringBuilder(fileLines[topPosition]);
                }
                else
                {
                    topPosition++;
                    WriteLines(fileLines);
                    return;
                }
                DeleteBackspaceWriter(false);
                fileLines = SmallArrayString(fileLines, topPosition);
                WriteLines(fileLines);
                leftPosition = textBuilderFirst.Length;
                return;
            }
            DeleteBackspaceWriter(true);
            leftPosition--;
            return;
        }
        private void DeleteBackspaceWriter(bool withWriteLines)
        {
            allTextBuilder.Append(textBuilderFirst);
            allTextBuilder.Append(textBuilderEnd);
            fileLines[topPosition] = Convert.ToString(allTextBuilder);
            if (withWriteLines)
            {
                WriteLines(fileLines);
            }
        }
        private void Delete()
        {
            allTextBuilder.Clear();
            Console.Clear();
            if (textBuilderEnd.Length != 0)
            {
                textBuilderEnd.Remove(0, 1);
            }
            else
            {
                if (topPosition < fileLines.Length - 1)
                {
                    textBuilderEnd = new StringBuilder(fileLines[topPosition + 1]);
                }
                else
                {
                    WriteLines(fileLines);
                    return;
                }
                DeleteBackspaceWriter(false);
                fileLines = SmallArrayString(fileLines, topPosition);
                WriteLines(fileLines);
                return;
            }
            DeleteBackspaceWriter(true);
            return;
        }
        private void Enter()
        {
            Console.Clear();
            fileLines[topPosition] = Convert.ToString(textBuilderFirst);
            textBuilderFirst.Clear();
            topPosition++;
            leftPosition = 0;
            fileLines = BigArrayString(fileLines, topPosition);
            fileLines[topPosition] = Convert.ToString(textBuilderEnd);
            WriteLines(fileLines);
            return;
        }
        private void ElseClick(ConsoleKeyInfo key)
        {
            allTextBuilder.Clear();
            Console.Clear();
            textBuilderFirst.Append(key.KeyChar);
            DeleteBackspaceWriter(true);
            leftPosition++;
            return;
        }
        private static string[] BigArrayString(string[] fileText, int topPosition)
        {
            string[] bigArrayString = new string[fileText.Length + 1];
            for (int i = 0, j = 0; i < bigArrayString.Length; i++, j++)
            {
                if (i == topPosition)
                {
                    j--;
                    continue;
                }
                bigArrayString[i] = fileText[j];
            }
            return bigArrayString;
        }
        private static string[] SmallArrayString(string[] fileText, int topPosition)
        {
            string[] smallArrayString = new string[fileText.Length - 1];
            for (int i = 0, j = 0; i < smallArrayString.Length; i++, j++)
            {
                if (i == topPosition + 1)
                {
                    j++;
                }
                smallArrayString[i] = fileText[j];
            }
            return smallArrayString;
        }
    }
}
