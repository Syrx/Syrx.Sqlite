namespace Syrx.Commanders.Databases.Settings
{
    /// <summary>
    /// Represents the resolved configuration for a single repository method command.
    /// </summary>
    public sealed record CommandSetting : ICommandSetting
    {
        /// <summary>
        /// Gets the split column used for Dapper multi-mapping operations.
        /// </summary>
        public string Split { get; init; } = "Id";

        /// <summary>
        /// Gets the SQL text or stored procedure name to execute.
        /// </summary>
        public required string CommandText { get; init; }

        /// <summary>
        /// Gets the command timeout, in seconds.
        /// </summary>
        public int CommandTimeout { get; init; } = 30;

        /// <summary>
        /// Gets the ADO.NET command type used for execution.
        /// </summary>
        public CommandType CommandType { get; init; } = CommandType.Text;

        /// <summary>
        /// Gets the Dapper command flags applied during execution.
        /// </summary>
        public CommandFlagSetting Flags { get; init; } = CommandFlagSetting.Buffered | CommandFlagSetting.NoCache;

        /// <summary>
        /// Gets the transaction isolation level used for execute operations.
        /// </summary>
        public IsolationLevel IsolationLevel { get; init; } = IsolationLevel.Serializable;

        /// <summary>
        /// Gets the connection alias used to resolve the underlying connection string.
        /// </summary>
        public required string ConnectionAlias { get; init; }
    }
}
