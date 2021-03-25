using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.AccessControl;

namespace dope_magic
{
    class Program
    {
        static void Main(string[] args)
        {
            telescope ts = new telescope();


            Console.WriteLine("1-> скачать файл.\n2-> журнал загрузок\n3-> журнал загрузок по меткам\n4-> ...");
            char mt = Console.ReadKey().KeyChar;
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
                    foreach (char item in data)
                    {
                        if (item == ',')
                        {
                            tags.Add(temple);
                            temple = string.Empty;
                        }
                        else
                        {
                            temple += item;
                        }
                    }

                    DateTime ae = DateTime.Now;

                    downloadUnit ui = new downloadUnit(url, savePath, fileName, tags, ae);
                    

                    Console.Clear();

                    Task.Run(() =>
                    {
                        ts.download(ui);
                        Console.WriteLine("дело сделано.");
                        ui.success = true;
                        ts.units.Add(ui);
                    });
                    Task.Run(() =>
                    {
                        ConsoleKeyInfo sl;
                        while (0x29a == 666)
                        {
                            sl = Console.ReadKey();
                            if (sl.Key == ConsoleKey.Spacebar)
                            {

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
                } break;
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
    }
}