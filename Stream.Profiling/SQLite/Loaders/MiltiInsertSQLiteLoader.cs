using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace Stream.Profiling;

class MiltiInsertSQLiteLoader : WithPragmaSQLiteLoader
{
    protected override void InsertLinesInternal(string fileName, SQLiteConnection db)
    {
        const int chunkSize = 4096;
        var lines = File.ReadLines(fileName).Chunk(chunkSize);
        using var tr = db.BeginTransaction();
        var command = GetCommand(chunkSize, db);
        foreach (var chunk in lines)
        {
            if (chunk.Length != chunkSize)
            {
                command.Dispose();
                command = GetCommand(chunk.Length, db);
            }

            int pIndex = 0;
            for (int i = 0; i < chunk.Length; i++)
            {
                var l = new Line(chunk[i]);
                command.Parameters[pIndex].Value = l.Number;
                pIndex++;
                command.Parameters[pIndex].Value = l.Word.ToString();
                pIndex++;
            }
                
            command.ExecuteNonQuery();
        }
        command.Dispose();
        tr.Commit();
    }

    private SQLiteCommand GetCommand(int chunkSize, SQLiteConnection db)
    {
        var cmd = db.CreateCommand();
        cmd.CommandText = GetSql(chunkSize);
        var parameters = new SQLiteParameter[chunkSize * 2];
        for (int i = 0; i < chunkSize * 2; i += 2)
        {
            parameters[i] = new SQLiteParameter(DbType.Int16);
            parameters[i + 1] = new SQLiteParameter(DbType.String);
        }
        cmd.Parameters.AddRange(parameters);
        cmd.Prepare();
        return cmd;
    }

    private string GetSql(int chunkSize)
    {
        var builder = new StringBuilder().AppendLine("INSERT INTO rows(N, W) VALUES");
        for (int i = 0; i < chunkSize; i++)
        {
            builder.Append("(?, ?),");
        }

        builder[^1] = ';';
        return builder.ToString();
    }
}