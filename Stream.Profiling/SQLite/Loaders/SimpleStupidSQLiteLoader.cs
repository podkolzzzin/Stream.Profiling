using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Stream.Profiling;

class SimpleStupidSQLiteLoader : IDisposable
{
    public SimpleStupidSQLiteLoader()
    {
            
    }

    public void InsertLines(string fileName)
    {
        File.Delete(DbName);
        using var db = new SQLiteConnection(GetConnectionString());
        db.Open();
        SetupDatabase(db);

        InsertLinesInternal(fileName, db);
    }

    protected virtual void InsertLinesInternal(string fileName, SQLiteConnection db)
    {
        foreach (var line in File.ReadLines(fileName))
        {
            var l = new Line(line);
            var cmd = GetCommand(db);
            try
            {
                cmd.Parameters[0].Value = l.Number;
                cmd.Parameters[1].Value = l.Word.ToString();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                FinalizeCommand(cmd);
            }
        }
    }

    protected virtual void FinalizeCommand(SQLiteCommand command) => command.Dispose();

    protected virtual SQLiteCommand GetCommand(SQLiteConnection connection)
    {
        var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO rows(N, W) VALUES(?, ?)";
        cmd.Parameters.Add(new SQLiteParameter(DbType.Int16));
        cmd.Parameters.Add(new SQLiteParameter(DbType.String));
        return cmd;
    }
        
    protected virtual string GetCreateQuery()
    {
        return "CREATE TABLE rows(N SMALLINT, W VARCHAR(101));";
    }

    private void SetupDatabase(SQLiteConnection db)
    {
        using var cmd = db.CreateCommand();
        cmd.CommandText = GetCreateQuery();
        cmd.ExecuteNonQuery();
    }

    public virtual string DbName => "sqlite.db";
        
    protected virtual string GetConnectionString() => $"Data Source={DbName}";

    public virtual void Dispose()
    {
    }
}