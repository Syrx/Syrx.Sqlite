//  ============================================================================================================================= 
//  author       : david sexton (@sextondjc | sextondjc.com)
//  date         : 2017.10.15 (17:58)
//  licence      : This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
//  =============================================================================================================================

namespace Syrx.Commanders.Databases.Connectors
{
    /// <summary>
    /// Default implementation of <see cref="IDatabaseConnector"/> which
    /// resolves connection string settings from <see cref="ICommanderSettings"/>
    /// and creates provider-specific <see cref="IDbConnection"/> instances using
    /// a supplied provider factory predicate.
    /// </summary>
    /// <remarks>
    /// The connector caches connection string lookups by alias to avoid
    /// repeated LINQ searches on the settings collection which can be
    /// expensive in high-throughput scenarios.
    /// </remarks>
    public class DatabaseConnector : IDatabaseConnector
    {
        private readonly Func<DbProviderFactory> _providerPredicate;
        private readonly Dictionary<string, ConnectionStringSetting> _connectionsByAlias = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, ConnectionStringSetting> _connectionCache = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnector"/> class.
        /// </summary>
        /// <param name="settings">
        /// The commander settings containing configured connection aliases and connection strings.
        /// </param>
        /// <param name="providerPredicate">
        /// A delegate that returns the <see cref="DbProviderFactory"/> used to create database connections.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="settings"/> or <paramref name="providerPredicate"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when duplicate connection aliases are found in <paramref name="settings"/>.
        /// </exception>
        public DatabaseConnector(
            ICommanderSettings settings,
            Func<DbProviderFactory> providerPredicate
        )
        {
            Throw<ArgumentNullException>(settings != null, nameof(settings));
            Throw<ArgumentNullException>(providerPredicate != null, nameof(providerPredicate));

            foreach (var connection in settings!.Connections ?? Enumerable.Empty<ConnectionStringSetting>())
            {
                Throw<ArgumentException>(
                    _connectionsByAlias.TryAdd(connection.Alias, connection),
                    $"Duplicate connection alias '{connection.Alias}' was found in settings.");
            }

            _providerPredicate = providerPredicate!;
        }

        /// <summary>
        /// Gets a <see cref="ConnectionStringSetting"/> for the provided alias.
        /// Uses a thread-safe cache to avoid repeated enumeration of the settings
        /// collection.
        /// </summary>
        /// <param name="connectionAlias">The connection alias to lookup.</param>
        /// <returns>The cached or newly retrieved <see cref="ConnectionStringSetting"/>, or <c>null</c> if not found.</returns>
        private ConnectionStringSetting GetConnectionSetting(string connectionAlias) =>
            _connectionCache.GetOrAdd(connectionAlias, alias =>
                _connectionsByAlias.TryGetValue(alias, out var setting) ? setting : null!);

        /// <summary>
        /// Creates and returns a new <see cref="IDbConnection"/> configured with
        /// the connection string defined by the provided <paramref name="setting"/>.
        /// </summary>
        /// <param name="setting">The <see cref="CommandSetting"/> used to determine the connection alias.</param>
        /// <returns>A new openable <see cref="IDbConnection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="setting"/> is <c>null</c>.</exception>
        /// <exception cref="NullReferenceException">If no matching connection alias is present in settings, or the provider failed to create a connection.</exception>
        public IDbConnection CreateConnection(CommandSetting setting)
        {
            Throw<ArgumentNullException>(setting != null, nameof(setting));

            var connectionStringSetting = GetConnectionSetting(setting?.ConnectionAlias!);
            Throw<NullReferenceException>(connectionStringSetting != null, Messages.NoAliasedConnection, setting?.ConnectionAlias!);

            var connection = _providerPredicate.Invoke().CreateConnection();
            Throw<NullReferenceException>(connection != null, Messages.NoConnectionCreated, setting?.ConnectionAlias!);

            // assign the connection and return
            connection!.ConnectionString = connectionStringSetting?.ConnectionString;
            return connection;
        }
        
        private static class Messages
        {
            internal const string NoAliasedConnection =
                "There is no connection with the alias '{0}' in the settings. Please check settings.";

            internal const string NoConnectionCreated =
                "The provider predicate did not return a connection for the aliased connection '{0}'.";
        }
    }



}
