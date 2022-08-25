using System;
using System.Data.SQLite;

namespace Stream.Profiling;

class PreparedQuerySQLiteLoader : WithPragmaSQLiteLoader, IDisposable
{
    private SQLiteCommand _command;
        
    protected override SQLiteCommand GetCommand(SQLiteConnection connection)
    {
        if (_command == null)
        {
            _command = base.GetCommand(connection);
            _command.Prepare();
        }

        return _command;
    }

    protected override void FinalizeCommand(SQLiteCommand command)
    {
    }

    public override void Dispose()
    {
        _command?.Dispose();
    }
}