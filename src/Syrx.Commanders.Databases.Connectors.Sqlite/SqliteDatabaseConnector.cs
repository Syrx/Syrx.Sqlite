using Microsoft.Data.Sqlite;
using Syrx.Commanders.Databases.Settings;

namespace Syrx.Commanders.Databases.Connectors.Sqlite
{
    /// <summary>
    /// Creates SQLite database connections for Syrx command execution.
    /// </summary>
    /// <param name="settings">The Syrx command and connection settings used to resolve SQLite connections.</param>
    public class SqliteDatabaseConnector(ICommanderSettings settings) : DatabaseConnector(settings, () => SqliteFactory.Instance)
    {
    }
}
