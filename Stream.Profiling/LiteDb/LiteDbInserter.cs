using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace Stream.Profiling.LiteDb;

public struct LiteDbLine
{
    public string Word { get; set; }
    public int Number { get; set; }
}

public class LiteDbLoader : IDisposable
{
    private readonly int _chunkSize;
    private readonly LiteDatabase _db;

    public LiteDbLoader(int chunkSize)
    {
        File.Delete(DbName);
        _chunkSize = chunkSize;
        _db = new LiteDatabase(DbName);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    public void InsertLines(string fileName)
    {
        var collection = _db.GetCollection<LiteDbLine>();
        var lines = new LiteDbLine[_chunkSize];
        foreach (var v in File.ReadLines(fileName).Chunk(_chunkSize))
        {
            if (lines.Length != v.Length)
                lines = new LiteDbLine[v.Length];
            
            for (int i = 0; i < v.Length; i++)
            {
                var l = new Line(v[i]);
                lines[i] = new LiteDbLine() { Number = l.Number, Word = l.Word.ToString() };
            }

            collection.InsertBulk(lines, v.Length);
        }
    }

    public string Name => "litedb " + _chunkSize;
    public string DbName { get; } = "litedb.db";

}