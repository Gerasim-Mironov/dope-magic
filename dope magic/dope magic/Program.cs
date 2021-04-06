using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.AccessControl;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace dope_magic
{
    class Program
    {
        static void Main(string[] args)
        {
            telescope ts = new telescope();

            while (true)
            {
                Console.Clear();

                Console.WriteLine("1-> скачать файл.\n2-> журнал загрузок\n3-> журнал загрузок по меткам\n4-> управление загруженными файлами.");
                char mt = Console.ReadKey().KeyChar;
                Console.Clear();
                switch (mt)
                {
                    case '1':
                        {
                            Console.WriteLine("ссылка:");
                            string url = Console.ReadLine();
                            Console.WriteLine("название файла:");
                            string fileName = Console.ReadLine();
                            Console.WriteLine("место захоронения файла:");
                            string savePath = Console.ReadLine();

                            Console.WriteLine("метки (через запятую)(можно и без них):");
                            string data = Console.ReadLine();
                            string temple = string.Empty;
                            List<string> tags = new List<string>();
                            for (int i = 0; i < data.Length; i++)
                            {
                                if (data[i] == ',' || i == data.Length)
                                {
                                    tags.Add(temple);
                                    temple = string.Empty;
                                }
                                else if(data[i] == ' ')
                                {
                                    ;
                                }
                                else
                                {
                                    temple += data[i];
                                }
                            }

                            DateTime ae = DateTime.Now;

                            downloadUnit ui = new downloadUnit(url, savePath, fileName, tags, ae);


                            Console.Clear();

                            Task re = Task.Run(() =>
                            {
                                ts.download(ui);
                                Console.WriteLine("дело сделано.");
                                ui.success = true;
                                ts.units.Add(ui);
                            });
                            Task.Run(() =>
                            {
                                while (!re.IsCompleted)
                                {
                                    bool pause = false;
                                    ConsoleKeyInfo key;
                                    key = Console.ReadKey();
                                    if (key.Key == ConsoleKey.Spacebar)
                                    {
                                        if (pause == false)
                                        {
                                            re.Wait();
                                            pause = true;
                                        }
                                        else
                                        {
                                            re = Task.Factory.StartNew(() =>
                                            {
                                                ts.download(ui);
                                                Console.WriteLine("дело сделано.");
                                                ui.success = true;
                                                ts.units.Add(ui);
                                            });
                                            pause = false;
                                        }
                                    }
                                }
                            });

                            Console.ReadKey();
                        } break;
                    case '2':
                        {
                            Console.WriteLine("успешно:");
                            foreach (downloadUnit item in ts.units)
                            {
                                if (item.success == true)
                                {
                                    Console.WriteLine($"{item.fileName} ({item.url}), {item.unl}");
                                }
                            }
                            Console.WriteLine("провалено:");
                            foreach (downloadUnit item in ts.units)
                            {
                                if (item.success == false)
                                {
                                    Console.WriteLine($"{item.fileName} ({item.url}), {item.unl}");
                                }
                            }

                            Console.ReadKey();
                        } break;
                    case '3':
                        {
                            Console.WriteLine("метки (через запятую):");
                            string income = Console.ReadLine();
                            List<string> vs = new List<string>();

                            string msg = string.Empty;
                            for (int i = 0; i < income.Length; i++)
                            {
                                if (income[i] == ',' || i == income.Length)
                                {
                                    vs.Add(msg);
                                    msg = string.Empty;
                                }
                                else if(income[i] == ' ')
                                {
                                    ;
                                }
                                else
                                {
                                    msg += income[i];
                                }
                            }

                            foreach (string item in vs)
                            {
                                foreach(downloadUnit obj in ts.units)
                                {
                                    foreach(string sbj in obj.tags)
                                    {
                                        if(sbj == item)
                                        {
                                            Console.WriteLine($"{obj.fileName} ({obj.url}), {obj.unl}");
                                        }
                                    }
                                }
                            }

                            Console.ReadKey();
                        } break;
                    case '4':
                        {
                            Console.WriteLine("работа с загрузками.\n");

                            List<string> weight = new List<string>();
                            foreach(downloadUnit unit in ts.units)
                            {
                                weight.Add($"{unit.savePath}\\{unit.fileName}");
                            }

                            while (true)
                            {
                                int unl = graphicMenu.VerticalMenu(weight.ToArray());
                                if (unl == -2)
                                    break;

                                #region newUnit
                                string url = ts.units[unl].url;
                                string savePath = ts.units[unl].savePath;
                                string fileName = ts.units[unl].fileName;
                                List<string> tags = ts.units[unl].tags;
                                DateTime dv = ts.units[unl].unl;
                                bool? success = ts.units[unl].success;
                                #endregion

                                Console.Clear();

                                FileInfo vh = new FileInfo(weight[unl]);

                                Console.WriteLine("1-> переместить\n2-> переименовать\n3-> удалить");
                                char ch = Console.ReadKey().KeyChar;
                                Console.Clear();
                                switch (ch)
                                {
                                    case '1':
                                        {
                                            Console.WriteLine("новое место жительства:");
                                            string path = Console.ReadLine();

                                            File.Move(vh.FullName, path + "\\" + vh.Name);

                                            downloadUnit temp = new downloadUnit(url, path, fileName, tags, dv, success);
                                            ts.units[unl] = temp;

                                            Console.WriteLine("дело сделано.");
                                        }
                                        break;
                                    case '2':
                                        {
                                            Console.WriteLine("новое имя:");
                                            string tempName = Console.ReadLine() + vh.Extension;
                                            string newName = vh.DirectoryName + "\\" + tempName;

                                            File.Move(vh.FullName, newName);

                                            downloadUnit temp = new downloadUnit(url, savePath, tempName, tags, dv, success);
                                            ts.units[unl] = temp;

                                            Console.WriteLine("дело сделано");
                                        }
                                        break;
                                    case '3':
                                        {
                                            File.Delete(vh.FullName);
                                            ts.units.RemoveAt(unl);

                                            Console.WriteLine("дело сделано.");
                                        }
                                        break;
                                }

                                Console.Clear();
                            }
                        }break;
                    default:
                        {
                            Console.WriteLine("неправильно!");
                        }break;
                }
            }
            

        }
    }


    class telescope
    {
        public List<downloadUnit> units = new List<downloadUnit>();
        async public Task download(downloadUnit re)
        {
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadFile(re.url, re.savePath + "\\" + re.fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ну ты и {e.Message}");
                //return false;
            }

            //return true;
        }
    }

    public class downloadUnit
    {
        public string url { get; set; }
        public string savePath { get; set; }
        public string fileName { get; set; }

        public List<string> tags { get; set; }

        public DateTime unl { get; set; }

        public bool? success;

        public downloadUnit()
        {
            tags = new List<string>();
        }

        public downloadUnit(string url, string savePath, string fileName, List<string> tags, DateTime unl)
        {
            this.url = url;
            this.savePath = savePath;
            this.fileName = fileName;
            this.tags = tags;
            this.unl = unl;
        }
        public downloadUnit(string url, string savePath, string fileName, List<string> tags, DateTime unl, bool? success)
        {
            this.url = url;
            this.savePath = savePath;
            this.fileName = fileName;
            this.tags = tags;
            this.unl = unl;
            this.success = success;
        }
    }



    #region gololobov
    class graphicMenu
    {
        public static int VerticalMenu(string[] elements)
        {
            int maxLen = 0;
            foreach (var item in elements)
            {
                if (item.Length > maxLen)
                    maxLen = item.Length;
            }
            ConsoleColor bg = Console.BackgroundColor;
            ConsoleColor fg = Console.ForegroundColor;
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.CursorVisible = false;
            int pos = 0;
            while (true)
            {

                for (int i = 0; i < elements.Length; i++)
                {
                    Console.SetCursorPosition(x, y + i);
                    if (i == pos)
                    {
                        Console.BackgroundColor = fg;
                        Console.ForegroundColor = bg;
                    }
                    else
                    {
                        Console.BackgroundColor = bg;
                        Console.ForegroundColor = fg;
                    }
                    Console.Write(elements[i].PadRight(maxLen));
                }

                ConsoleKey consoleKey = Console.ReadKey().Key;
                switch (consoleKey)
                {

                    case ConsoleKey.Enter:
                        return pos;
                        break;

                    case ConsoleKey.Escape:
                        return -2;
                        break;

                    case ConsoleKey.UpArrow:
                        if (pos > 0)
                            pos--;
                        break;

                    case ConsoleKey.DownArrow:
                        if (pos < elements.Length - 1)
                            pos++;
                        break;


                    default:
                        break;
                }

            }
        }
    }
    #endregion
}