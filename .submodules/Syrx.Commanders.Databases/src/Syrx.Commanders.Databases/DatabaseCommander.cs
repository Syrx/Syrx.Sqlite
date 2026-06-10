//  ============================================================================================================================= 
//  author       : david sexton (@sextondjc | sextondjc.com)
//  date         : 2017.10.15 (17:58)
//  licence      : This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
//  =============================================================================================================================

namespace Syrx.Commanders.Databases
{
    /// <summary>
    /// Database-backed implementation of <see cref="ICommander{TRepository}"/>.
    /// Responsible for resolving configured commands and executing them against
    /// a database using the supplied <see cref="IDatabaseConnector"/>.
    /// </summary>
    /// <typeparam name="TRepository">The repository type whose methods this commander executes.</typeparam>
    public partial class DatabaseCommander<TRepository> : ICommander<TRepository>
    {
        private readonly IDatabaseConnector _connector;
        private readonly IDatabaseCommandReader _reader;
        private readonly Type _type;
        private readonly string _typeFullName;
        private readonly ConcurrentDictionary<string, CommandSetting> _commandCache = new();

        /// <summary>
        /// Releases any resources held by the commander. Currently a no-op
        /// because the implementation does not hold unmanaged resources.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Creates a new instance of <see cref="DatabaseCommander{TRepository}"/>.
        /// </summary>
        /// <param name="reader">The command reader used to resolve command settings.</param>
        /// <param name="connector">The connector used to create database connections.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="reader"/> or <paramref name="connector"/> is <c>null</c>.</exception>
        public DatabaseCommander(IDatabaseCommandReader reader, IDatabaseConnector connector)
        {
            _reader = reader;
            _connector = connector;
            _type = typeof(TRepository);
            _typeFullName = _type.FullName!;
        }

        /// <summary>
        /// Gets a CommandSetting using thread-safe caching to eliminate repetitive lookups.
        /// This provides significant performance improvements by caching the results of
        /// expensive LINQ operations in DatabaseCommandReader.GetCommand().
        /// Uses pre-computed _typeFullName to avoid reflection overhead on every call.
        /// </summary>
        /// <param name="method">The method name from [CallerMemberName]</param>
        /// <returns>The cached or newly retrieved CommandSetting</returns>
        [Browsable(false)]
        private CommandSetting GetCommandSetting(string method)
        {
            var cacheKey = $"{_typeFullName}.{method}";
            return _commandCache.GetOrAdd(cacheKey, _ => _reader.GetCommand(_type, method));
        }

        [Browsable(false)]
        private CommandDefinition GetCommandDefinition(
            CommandSetting setting,
            object parameters = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return new CommandDefinition(
                commandText: setting.CommandText,
                parameters: parameters,
                transaction: transaction,
                commandTimeout: setting.CommandTimeout,
                commandType: setting.CommandType,
                flags: (CommandFlags) setting.Flags,
                cancellationToken: cancellationToken);
        }
    }
}
