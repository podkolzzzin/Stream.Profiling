namespace Stream.Profiling;

class WithPragmaSQLiteLoader : TransactionedSQLiteLoader
{
    protected override string GetCreateQuery()
    {
        return base.GetCreateQuery() 
               + "PRAGMA synchronous = OFF;"
               + "PRAGMA journal_mode = OFF;"
               + "PRAGMA temp_store = MEMORY;"
               + "PRAGMA locking_mode = exclusive;";
    }
}