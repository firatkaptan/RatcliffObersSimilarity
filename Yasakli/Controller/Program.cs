using System;
using System.Collections.Generic;
using System.Diagnostics;


class Program
{
    private const double Ratio = 85.0;
    static void Main(string[] args)
    {
        Stopwatch stopWatch = new Stopwatch();
        BKTree bkTree = new BKTree(Ratio, 8);
        DBHelper dbHelper = new DBHelper();
        TimeSpan timeSpan;
        List<Match> result;
        dbHelper.afterProgress += ProgressEvent;


        Console.WriteLine("Veriler alınıyor...");

        dbHelper.LoadFromFile(bkTree);

        Console.WriteLine("\nHazır!");

        dbHelper = null;

        while (true)
        {
            GC.Collect();
            try
            {
                string input = Console.ReadLine();
                stopWatch.Start();
                string[] array = input.Split(' ');
                if (array.Length == 2 && array[0] == "add")
                {
                    input = ValidString(String.Join("", array).Substring(3));
                    bkTree.Add(input);
                    bkTree.CheckAndRemoveCache(input);
                    Console.WriteLine("Eklendi: " + input);
                }
                else
                {
                    result = bkTree.Search(ValidString(input));
                    if (result.Count > 0)
                    {
                        Console.WriteLine("Eşleşmeler: " + String.Join(", ", result.ToArray()));
                        Console.WriteLine("Döngü sayısı: " + bkTree.src.count);
                    }                        
                    else
                        Console.WriteLine("Eşleşme bulunamadı.");
                }
                stopWatch.Stop();
                timeSpan = stopWatch.Elapsed;

                Console.WriteLine("Süre: " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10));
            }
            catch (Exception ex)
            {
                stopWatch.Stop();
                Console.WriteLine("Hata Oluştu: " + ex.Message);
            }
            finally
            {
                stopWatch.Reset();
            }
        }
    }
    private static void ProgressEvent(int i)
    {
        Console.Write("\rYükleniyor: {0}%", i);
    }

    private static string ValidString(string str)
    {
        str = str.ToUpper();
        str = str.Trim();
        str = str.Replace(" ", "");
        str = str.Replace("Ü", "U");
        str = str.Replace("İ", "I");
        str = str.Replace("Ç", "C");
        str = str.Replace("Ş", "S");
        str = str.Replace("Ö", "O");
        str = str.Replace("Ğ", "G");
        return str;
    }
}

