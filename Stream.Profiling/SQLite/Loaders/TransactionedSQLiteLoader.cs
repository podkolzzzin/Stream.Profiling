using System.Data.SQLite;

namespace Stream.Profiling;

class TransactionedSQLiteLoader : SimpleStupidSQLiteLoader
{
    protected override void InsertLinesInternal(string fileName, SQLiteConnection db)
    {
        using var tr = db.BeginTransaction();
        base.InsertLinesInternal(fileName, db);
        tr.Commit();
    }
}