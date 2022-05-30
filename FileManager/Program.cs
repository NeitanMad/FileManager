using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileManager
{
    class Program
    {
        const int WINDOW_HEIGHT = 35;
        const int WINDOW_WIDHT = 120;
        private static string currentDir = Directory.GetCurrentDirectory();
        static string path = Path.Combine(Directory.GetCurrentDirectory(), "Errors.txt"); // Получение пути файла с ошибками
        public static List<string> list = new List<string>();

        static void Main(string[] args)
        {
            Console.Title = "File manager";

            Console.SetWindowSize(WINDOW_WIDHT, WINDOW_HEIGHT);
            Console.SetBufferSize(WINDOW_WIDHT, WINDOW_HEIGHT);

            DrawWindow(0, 0, WINDOW_WIDHT, 18);
            Information();

            UpdateConsole();

            Console.ReadKey(true);
        }

        /// <summary>
        /// Отрисовка окон
        /// </summary>
        /// <param name="x">Позиция курсора по оси Х</param>
        /// <param name="y">Позиция курсора по оси У</param>
        /// <param name="widht">Ширина окна</param>
        /// <param name="height">Высота окна</param>
        static void DrawWindow(int x, int y, int widht, int height) // x,y - координата курсора, widht,height - размер окна
        {
            Console.SetCursorPosition(x, y); // установка позиции курсора

            // верхняя рамка
            Console.Write("╔");
            for (int i = 0; i < widht - 2; i++) // ширрина -2 т.к два уголка
            {
                Console.Write("═");
            }
            Console.Write("╗");

            // боковые рамки
            Console.SetCursorPosition(x, y + 1); // у+1 т.к. переход на новую строку
            for (int i = 0; i < height - 2; i++) // высота -2 т.к есть верхняя и нижняя рамка
            {
                Console.Write("║"); // левая рамка
                for (int j = x + 1; j < x + widht - 1; j++) // j = x+1 - отступ слева , widh - 1 - оставляем место для правой рамки
                {
                    Console.Write(" ");
                }
                Console.Write("║");
            }

            // нижняя рамка
            Console.Write("╚");
            for (int i = 0; i < widht - 2; i++) // ширрина -2 т.к два уголка
            {
                Console.Write("═");
            }
            Console.Write("╝");
            Console.SetCursorPosition(x, y);
        }

        /// <summary>
        /// Отрисовка консоли
        /// </summary>
        /// <param name="x">Позиция курсора по оси Х</param>
        /// <param name="y">Позиция курсора по оси У</param>
        /// <param name="widht">Ширина окна</param>
        /// <param name="height">Высота окна</param>
        /// <param name="dir">Начальная директория</param>
        static void DrawConsole(int x, int y, int widht, int height, string dir)
        {
            DrawWindow(x, y, widht, height);
            Console.SetCursorPosition(x + 1, y + height / 2); // помещаем курсор в середину рамки, height / 2 - середина рамки по высоте
            Console.Write($"{dir}>"); // вывод начальной директории 
        }

        /// <summary>
        /// Отрисовка дерева каталогов
        /// </summary>
        /// <param name="dir">Дирректория</param>
        /// <param name="page">Номер страницы</param>
        static void DrawTree(DirectoryInfo dir, int page)
        {
            StringBuilder tree = new StringBuilder();
            GetTree(tree, dir, "", true);

            DrawWindow(0, 0, WINDOW_WIDHT, 18);
            (int currentLeft, int currentTop) = GetCursorPosition();
            int pageLines = 16;
            string[] lines = tree.ToString().Split(new char[] { '\n' });
            int pageTotal = (lines.Length + pageLines - 1) / pageLines;
            if (page > pageTotal)
                page = pageTotal;

            for (int i = (page - 1) * pageLines, counter = 0; i < page * pageLines; i++, counter++)
            {
                if (lines.Length - 1 > i)
                {
                    Console.SetCursorPosition(currentLeft + 1, currentTop + 1 + counter);
                    Console.WriteLine(lines[i]);
                }
            }

            //footer
            string footer = $"╡ {page} of {pageTotal} ╞";
            Console.SetCursorPosition(WINDOW_WIDHT / 2 - footer.Length / 2, 17);
            Console.WriteLine(footer);
        }

        /// <summary>
        /// Вывод ошибки в информационное окно
        /// </summary>
        /// <param name="e">Полученная ошибка</param>
        static void DrawWindowExeception(Exception e)
        {
            DrawWindow(0, 18, WINDOW_WIDHT, 10);
            (int currenLeft, int currentTop) = GetCursorPosition();
            int n = 0; // счетчик строк
            Console.SetCursorPosition(currenLeft + 1, currentTop + 1);
            char[] message = e.Message.ToCharArray();

            for (int i = 0; i < message.Length; i++)
            {
                Console.Write(message[i]);
                (int currenLeftt, int currentTopp) = GetCursorPosition();
                if (currenLeftt == WINDOW_WIDHT - 2)
                {
                    n++;
                    Console.SetCursorPosition(currenLeft + 1, currentTopp + n);
                }
            }

            DrawConsole(0, 28, WINDOW_WIDHT, 3, Properties.Settings.Default.StartUpCurrentDir);
        }

        /// <summary>
        /// Сохранение дерева в StringBuilder
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="dir"></param>
        /// <param name="indent"></param>
        /// <param name="lastDirectory"></param>
        static void GetTree(StringBuilder tree, DirectoryInfo dir, string indent, bool lastDirectory)
        {
            tree.Append(indent);
            if (lastDirectory)
            {
                tree.Append("└─");
                indent += "  ";
            }
            else
            {
                tree.Append("├─");
                indent += "│ ";
            }

            tree.Append($"{dir.Name}\n"); //<---------------------- ПЕРЕХОД НА СЛЕД СТРОКУ

            // Добавляем отображение файлов
            FileInfo[] subFiles = dir.GetFiles();
            for (int i = 0; i < subFiles.Length; i++)
            {
                if (i == subFiles.Length - 1)
                {
                    tree.Append($"{indent}└─{subFiles[i].Name}\n");
                }
                else
                {
                    tree.Append($"{indent}├─{subFiles[i].Name}\n");
                }
            }

            DirectoryInfo[] subDirects = dir.GetDirectories();
            for (int i = 0; i < subDirects.Length; i++)
                GetTree(tree, subDirects[i], indent, i == subDirects.Length - 1);
        }

        /// <summary>
        /// Вспомогательный метод для определения положения
        /// курсора во время ввода команды(команда вводится посимвольно)
        /// </summary>
        /// <returns></returns>
        static (int left, int top) GetCursorPosition()
        {
            return (Console.CursorLeft, Console.CursorTop);
        }

        /// <summary>
        /// Метод ввода команды
        /// </summary>
        /// <param name="widht"></param>
        static void EnterComand(int widht)
        {
            (int left, int top) = GetCursorPosition();
            StringBuilder command = new StringBuilder(); // динамическая строка
            string HistoryCommand = "";
            char key1;  
            int KeyCount = 0;

            do
            {
                var key = Console.ReadKey();
                key1 = ((char)key.Key);
                if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.UpArrow && key.Key != ConsoleKey.DownArrow)
                    command.Append(key.KeyChar);

                (int currentLeft, int currentTop) = GetCursorPosition(); // фиксация курсора после каждого ввода символа

                if (currentLeft == widht - 2) // если курсор выходит за рамки
                {
                    Console.SetCursorPosition(currentLeft - 1, top); // отодвигаем курсор на 1 символ влево
                    Console.Write(" "); // затираем введеныйсимвол пробелом
                    Console.SetCursorPosition(currentLeft - 1, top); // 
                }

                if (key.Key == ConsoleKey.Backspace) // действие при нажатии Backspace
                {
                    if (command.Length > 0)
                        command.Remove(command.Length - 1, 1);
                    if (currentLeft >= left)
                    {
                        Console.SetCursorPosition(currentLeft, top);
                        Console.Write(" ");
                        Console.SetCursorPosition(currentLeft, top);
                    }
                    else
                    {
                        Console.SetCursorPosition(left, top); // не дает затереть директорию
                    }

                }

                if (key.Key == ConsoleKey.DownArrow && list.Count > 0)
                {
                    DrawConsole(0, 28, WINDOW_WIDHT, 3, Properties.Settings.Default.StartUpCurrentDir);
                    try
                    {
                        if (KeyCount < list.Count)
                            KeyCount++;
                        Console.SetCursorPosition(currentLeft - 1, top);
                        Console.Write(list[list.Count - KeyCount]);
                        Console.SetCursorPosition(left, top);
                        int i = list.Count - KeyCount;
                        HistoryCommand = list[i];


                    }
                    catch (Exception e)
                    {
                        DrawWindowExeception(e);
                        SaveErrorsFile(e);
                    }
                }

                if (key.Key == ConsoleKey.UpArrow && list.Count > 0)
                {
                    DrawConsole(0, 28, WINDOW_WIDHT, 3, Properties.Settings.Default.StartUpCurrentDir);
                    try
                    {
                        if (KeyCount <= list.Count)
                            KeyCount--;
                        Console.SetCursorPosition(currentLeft - 1, top);
                        Console.Write(list[list.Count - KeyCount]);
                        Console.SetCursorPosition(left, top);
                        int i = list.Count - KeyCount;
                        HistoryCommand = list[i];
                    }
                    catch (Exception e)
                    {
                        DrawWindowExeception(e);
                        SaveErrorsFile(e);
                    }
                }

            } while (key1 != (char)13); // пока не введут Enter

            if (command.Length > 0)
                list.Add(command.ToString());

            if (HistoryCommand.Length == 0)
            {
                ParseCommandString(command.ToString());
            }
            else
            {
                ParseCommandString(HistoryCommand);
            }
        }

        /// <summary>
        /// Обновление консоли(перерисовка)
        /// </summary>
        static void UpdateConsole()
        {
            if (Properties.Settings.Default.StartUpCurrentDir == "")
            {
                DrawConsole(0, 28, WINDOW_WIDHT, 3, currentDir);
                EnterComand(WINDOW_WIDHT);
            }

            else
            {
                DrawConsole(0, 28, WINDOW_WIDHT, 3, Properties.Settings.Default.StartUpCurrentDir);

                if (Properties.Settings.Default.StartUpTree != "")
                {
                    DrawTree(new DirectoryInfo(Properties.Settings.Default.StartUpTree), Properties.Settings.Default.StartUpPage);
                    DrawConsole(0, 28, WINDOW_WIDHT, 3, Properties.Settings.Default.StartUpCurrentDir);
                }

                EnterComand(WINDOW_WIDHT);
            }
        }


        /// <summary>
        /// Обработка введенной команды
        /// </summary>
        /// <param name="command"></param>
        static void ParseCommandString(string command)
        {

            string[] commandParams = command.ToLower().Split(' '); // перевод в нижний регистр и рразделение через пробел
            if (commandParams.Length > 0) // проверка на наличие команды как таковой (может быть нажат только Enter)
            {
                switch (commandParams[0]) // проверка команды 
                {
                    case "cd":

                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1])) // проверка существования второй части команды
                        {
                            currentDir = commandParams[1];
                            Properties.Settings.Default.StartUpCurrentDir = commandParams[1].ToString(); // save dir
                            Properties.Settings.Default.Save();

                        }
                        break;

                    case "ls":

                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                        {
                            if (commandParams.Length > 3 && commandParams[2] == "-p" && int.TryParse(commandParams[3], out int n))
                            {
                                try
                                {
                                    DrawTree(new DirectoryInfo(commandParams[1]), n);
                                    Properties.Settings.Default.StartUpPage = n;
                                    Properties.Settings.Default.StartUpTree = commandParams[1].ToString();
                                    Properties.Settings.Default.Save();
                                }
                                catch (Exception e)
                                {
                                    SaveErrorsFile(e);
                                    DrawWindowExeception(e);

                                }

                            }
                            else
                            {
                                try
                                {
                                    DrawTree(new DirectoryInfo(commandParams[1]), 1);
                                    Properties.Settings.Default.StartUpPage = 1;
                                    Properties.Settings.Default.StartUpTree = commandParams[1].ToString();
                                    Properties.Settings.Default.Save();
                                }
                                catch (Exception e)
                                {
                                    DrawWindowExeception(e);
                                    SaveErrorsFile(e);
                                }

                            }
                        }
                        break;

                    case "cp":

                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                        {
                            try
                            {
                                CopyDirectory(commandParams[1], commandParams[2], true);
                            }
                            catch (Exception e)
                            {
                                DrawWindowExeception(e);
                                SaveErrorsFile(e);
                            }
                        }

                        if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                        {
                            try
                            {
                                CopyFile(commandParams[1], commandParams[2]);

                            }
                            catch (Exception e)
                            {
                                DrawWindowExeception(e);
                                SaveErrorsFile(e);
                            }
                        }
                        break;

                    case "del":

                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                            DeleteFolder(commandParams[1]);

                        if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                            DeliteFile(commandParams[1]);

                        break;

                    case "info":

                        if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                            InfoFile(commandParams[1]);

                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                            InfoDir(commandParams[1]);

                        if (commandParams.Length == 1)
                            Information();

                        break;
                }
            }

            UpdateConsole();
        }

        /// <summary>
        /// Копирование файла
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newpath"></param>
        static void CopyFile(string path, string newpath)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
                fileInfo.CopyTo(newpath, true);
        }

        /// <summary>
        /// Копирование директории
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newpath"></param>
        /// <param name="recursive"></param>
        static void CopyDirectory(string path, string newpath, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(path);

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create new directory
            Directory.CreateDirectory(newpath);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(newpath, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(newpath, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        /// <param name="path"></param>
        static void DeliteFile(string path)
        {
            File.Delete(path);
        }

        /// <summary>
        /// Удаление дирректории
        /// </summary>
        /// <param name="path"></param>
        static void DeleteFolder(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] dirArr = dir.GetDirectories();
            FileInfo[] file = dir.GetFiles();
            foreach (FileInfo f in file)
            {
                f.Delete();
            }
            foreach (DirectoryInfo df in dirArr)
            {
                DeleteFolder(df.FullName);
            }
            if (dir.GetDirectories().Length == 0 && dir.GetFiles().Length == 0)
                dir.Delete();
        }

        /// <summary>
        /// Вывод информации о файле
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        static void InfoFile(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            DrawWindow(0, 18, WINDOW_WIDHT, 10);
            (int currenLeft, int currentTop) = GetCursorPosition();
            Console.SetCursorPosition(currenLeft + 1, currentTop + 1);
            Console.WriteLine($"Имя файла: " + fileInfo.Name);
            Console.SetCursorPosition(currenLeft + 1, currentTop + 2);
            Console.WriteLine($"Время создания: " + fileInfo.CreationTimeUtc);
            Console.SetCursorPosition(currenLeft + 1, currentTop + 3);
            Console.WriteLine($"Размер файла: " + fileInfo.Length + " байт");
            Console.SetCursorPosition(currenLeft + 1, currentTop + 4);
            Console.WriteLine($"Расширение файла: " + fileInfo.Extension);
        }

        /// <summary>
        /// Вывод информации о директории
        /// </summary>
        /// <param name="path">Путь к дирректории</param>
        static void InfoDir(string path)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(path);
            DrawWindow(0, 18, WINDOW_WIDHT, 10);
            (int currenLeft, int currentTop) = GetCursorPosition();
            Console.SetCursorPosition(currenLeft + 1, currentTop + 1);
            Console.WriteLine($"Имя каталога: " + dirinfo.Name);
            Console.SetCursorPosition(currenLeft + 1, currentTop + 2);
            Console.WriteLine($"Время создания: " + dirinfo.CreationTimeUtc);
            Console.SetCursorPosition(currenLeft + 1, currentTop + 3);
            Console.WriteLine($"Родительский каталог: " + dirinfo.Parent);
            Console.SetCursorPosition(currenLeft + 1, currentTop + 4);
            Console.WriteLine($"Корневая директория: " + dirinfo.Root);
            Console.SetCursorPosition(currenLeft + 1, currentTop + 5);
            Console.WriteLine($"Полный путь: " + dirinfo.FullName);
        }

        /// <summary>
        /// Запись ошибки в текстовый файл
        /// </summary>
        /// <param name="e">Полученная ошибка</param>
        static void SaveErrorsFile(Exception e)
        {
            using (FileStream file = new FileStream(path, FileMode.Append))
            using (StreamWriter stream = new StreamWriter(file))
                stream.WriteLine("* " + DateTime.Now + " -- " + e);
        }

        /// <summary>
        /// Вывод информации
        /// </summary>
        static void Information()
        {
            DrawWindow(0, 18, WINDOW_WIDHT, 10);
            (int currenLeft, int currentTop) = GetCursorPosition();
            string inf = "СПИСОК КОМАНД:";
            Console.SetCursorPosition(WINDOW_WIDHT / 2 - inf.Length / 2, currentTop + 1);
            Console.WriteLine(inf);
            Console.SetCursorPosition(currenLeft + 1, currentTop + 2);
            Console.WriteLine("* ИЗМЕНИТЬ ДИРЕКТОРИЮ: CD SOMEDIR:\\..");
            Console.SetCursorPosition(currenLeft + 1, currentTop + 3);
            Console.WriteLine("* КОПИРОВАТЬ ФАЙЛ/ПАПКУ: CP <<ПУТЬ ИЗ>> SOMEDIR:\\.. <<ПУТЬ В>> SOMEDIR:\\..");
            Console.SetCursorPosition(currenLeft + 1, currentTop + 4);
            Console.WriteLine("* УДАЛИТЬ ФАЙЛ/ПАПКУ: DEL <<ПУТЬ>> SOMEDIR:\\..");
            Console.SetCursorPosition(currenLeft + 1, currentTop + 5);
            Console.WriteLine("* ИНФОРМАЦИЯ О ФАЙЛЕ/ПАПКЕ ИЛИ ВЫЗОВ МЕНЮ: INFO <<ПУТЬ>> SOMEDIR:\\.. // INFO");
            Console.SetCursorPosition(currenLeft + 1, currentTop + 6);
            Console.WriteLine("* ОТРИСОВАТЬ ДЕРЕВО КАТАЛОГА: LS <<ПУТЬ>> SOMEDIR:\\.. <<СТРАНИЦА>> -P <<НОМЕР СТРАНИЦЫ>> ..");
            Console.SetCursorPosition(currenLeft + 1, currentTop + 7);
            Console.WriteLine("* ДВИЖЕНИЕ ПО ИСТОРИИ КОМАНД: СТРЕЛКА ВНИЗ ИЛИ СТРЕЛКА ВВЕРХ");
            Console.SetCursorPosition(currenLeft + 1, currentTop + 8);
            Console.WriteLine("** ДЛЯ КОРРЕКТНОГО ОТОБРАЖЕНИЯ КОНСОЛЬНОГО ПРИЛОЖЕНИЯ НЕ ИЗМЕНЯЙТЕ МАСШТАБ ОКНА!");
        }
    }
}
