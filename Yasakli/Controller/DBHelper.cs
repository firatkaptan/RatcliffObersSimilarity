using System;
using System.Data;
using System.Data.SqlClient;

class DBHelper
{
    public delegate void Progress(int i);
    public event Progress afterProgress;

    SqlConnection sqlConn;
    SqlCommand cmd;

    public void Open()
    {
        sqlConn = new SqlConnection("Persist Security Info=False;Integrated Security=true; Initial Catalog = Firat; Server = DEVSQL");
        sqlConn.Open();
    }

    public void Close()
    {
        sqlConn.Close();
    }

    public void writetoFile()
    {
        string[] firstnames = System.IO.File.ReadAllLines(@"C:\Users\Firat\Desktop\first.txt");
        string[] lastnames = System.IO.File.ReadAllLines(@"C:\Users\Firat\Desktop\last.txt");

        string[] names = new string[firstnames.Length * lastnames.Length];

        for (int i = 0; i < 2500; i++)
        {
            for (int j = 0; j < 2500; j++)
            {
                names[2500 * i + j] = ValidString(firstnames[i] + lastnames[j]);
            }
        }
        System.IO.File.WriteAllLines(@"C:\Users\Firat\Desktop\All.txt", names);
    }
    public void LoadFromFile(BKTree bk)
    {        
        try
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Firat\Desktop\All.txt");
            while ((line = file.ReadLine()) != null)
            {
                bk.Add(line);
            }

            file.Close();
        }
        catch (Exception ex)
        {
            throw new Exception("Ağaç oluşturulamadı. " + ex.Message);
        }
    }

    public void Load(BKTree bk)
    {
        try
        {
            DataTable dt = GetDataTable();
            int current, progress = -1;
            int rowCount = dt.Rows.Count;
            for (int i = rowCount - 1; i > -1; i--)
            {
                bk.Add(dt.Rows[i]["Ad"].ToString());
                dt.Rows.RemoveAt(i);

                current = 100 - i * 100 / rowCount;

                if (current != progress)
                {
                    progress = current;
                    afterProgress(progress);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Ağaç oluşturulamadı. " + ex.Message);
        }
    }

    private DataTable GetDataTable()
    {
        try
        {
            cmd = new SqlCommand("SELECT Ad FROM Firat..PERSONS", sqlConn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw new Exception("Veritabanından yasaklılar alınırken hata oluştu. " + ex.Message);
        }
    }
    private string ValidString(string str)
    {
        str = str.ToUpper();
        str = str.Replace("İ", "I");
        return str;
    }

}

