using System.Data.SQLite;
using System.IO;

namespace Stream.Profiling.SQLite.Sorters;

public class SimpleStupidSorter : ISorter
{
    private readonly string _dbFileName;

    public SimpleStupidSorter(string dbFileName)
    {
        _dbFileName = dbFileName;
    }
    
    public void Sort()
    {
        using var connection = new SQLiteConnection($"Data Source={_dbFileName}");
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT N, W FROM rows ORDER BY W, N";
        using var reader = cmd.ExecuteReader();
        using var writer = new StreamWriter("sorted.txt");
        while (reader.Read())
        {
            writer.Write(reader[0]);
            writer.Write(". ");
            writer.WriteLine(reader[1]);
        }
    }
}