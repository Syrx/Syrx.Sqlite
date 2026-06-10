namespace Syrx.Commanders.Databases.Settings.Extensions
{
    /// <summary>
    /// Builds a <see cref="CommandSetting"/> instance by collecting command execution metadata.
    /// </summary>
    public class CommandSettingBuilder
    {
        private string _split = "id"; // default
        private string _commandText;
        private string _alias;
        private int _commandTimeout = 30;
        private CommandType _commandType = CommandType.Text;
        private CommandFlagSetting _commandFlagSetting = CommandFlagSetting.Buffered | CommandFlagSetting.NoCache;
        private IsolationLevel _isolationLevel = IsolationLevel.Serializable;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSettingBuilder"/> class.
        /// </summary>
        public CommandSettingBuilder()
        {
            _commandText = string.Empty;
            _alias = string.Empty;
        }

        /// <summary>
        /// Sets the split column used for Dapper multi-mapping operations.
        /// </summary>
        /// <param name="split">The split column name. Defaults to <c>id</c>.</param>
        /// <returns>The current <see cref="CommandSettingBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="split"/> is <see langword="null"/>, empty, or whitespace.</exception>
        public CommandSettingBuilder SplitOn(string split = "id")
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(split), nameof(split));
            _split = split;
            return this;
        }

        /// <summary>
        /// Sets the SQL text or stored procedure name to execute.
        /// </summary>
        /// <param name="commandText">The SQL text or stored procedure name.</param>
        /// <returns>The current <see cref="CommandSettingBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="commandText"/> is <see langword="null"/>, empty, or whitespace.</exception>
        public CommandSettingBuilder UseCommandText(string commandText)
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(commandText), nameof(commandText));
            _commandText = commandText;
            return this;
        }

        /// <summary>
        /// Sets the command timeout in seconds.
        /// </summary>
        /// <param name="commandTimeout">The timeout in seconds. Must be greater than 1.</param>
        /// <returns>The current <see cref="CommandSettingBuilder"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="commandTimeout"/> is less than or equal to 1.</exception>
        public CommandSettingBuilder SetCommandTimeout(int commandTimeout = 30)
        {
            Throw<ArgumentException>(commandTimeout > 1, $"CommandTimeout cannot be less than 1. The value '{commandTimeout}' is not valid.");
            _commandTimeout = commandTimeout;
            return this;
        }

        /// <summary>
        /// Sets the ADO.NET command type used for execution.
        /// </summary>
        /// <param name="commandType">The command type to use. Defaults to <see cref="CommandType.Text"/>.</param>
        /// <returns>The current <see cref="CommandSettingBuilder"/> instance.</returns>
        public CommandSettingBuilder SetCommandType(CommandType commandType = CommandType.Text)
        {
            _commandType = commandType;
            return this;
        }

        /// <summary>
        /// Sets the Dapper command flags applied during execution.
        /// </summary>
        /// <param name="commandFlagSetting">The command flags to apply.</param>
        /// <returns>The current <see cref="CommandSettingBuilder"/> instance.</returns>
        public CommandSettingBuilder SetFlags(CommandFlagSetting commandFlagSetting = CommandFlagSetting.Buffered | CommandFlagSetting.NoCache)
        {
            _commandFlagSetting = commandFlagSetting;
            return this;
        }

        /// <summary>
        /// Sets the transaction isolation level used for execute operations.
        /// </summary>
        /// <param name="isolationLevel">The isolation level to apply.</param>
        /// <returns>The current <see cref="CommandSettingBuilder"/> instance.</returns>
        public CommandSettingBuilder SetIsolationLevel(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            _isolationLevel = isolationLevel;
            return this;
        }

        /// <summary>
        /// Sets the connection alias used to resolve the connection string for the command.
        /// </summary>
        /// <param name="alias">The connection alias to use.</param>
        /// <returns>The current <see cref="CommandSettingBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="alias"/> is <see langword="null"/>, empty, or whitespace.</exception>
        public CommandSettingBuilder UseConnectionAlias(string alias)
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(alias), nameof(alias));
            _alias = alias;
            return this;
        }

        protected internal CommandSetting Build()
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(_alias), $"The connection string alias must be set. Use the '{nameof(UseConnectionAlias)}' method to set.");
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(_commandText), $"The command text cannot be null, empty or blank. Use the '{nameof(UseCommandText)}' method to set.");

            return new CommandSetting
            {
                CommandText = _commandText,
                CommandTimeout = _commandTimeout,
                CommandType = _commandType,
                ConnectionAlias = _alias,
                Flags = _commandFlagSetting,
                IsolationLevel = _isolationLevel,
                Split = _split
            };
        }

    }

}