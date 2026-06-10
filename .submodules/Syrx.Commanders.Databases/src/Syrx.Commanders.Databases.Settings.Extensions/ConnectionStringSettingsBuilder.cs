namespace Syrx.Commanders.Databases.Settings.Extensions
{
    /// <summary>
    /// Builds a <see cref="ConnectionStringSetting"/> instance by collecting alias and connection string values.
    /// </summary>
    public class ConnectionStringSettingsBuilder
    {
        private string _alias;
        private string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringSettingsBuilder"/> class.
        /// </summary>
        public ConnectionStringSettingsBuilder()
        {
            _alias = string.Empty;
            _connectionString = string.Empty;
        }

        /// <summary>
        /// Sets the alias used to reference the connection string.
        /// </summary>
        /// <param name="alias">The alias to assign.</param>
        /// <returns>The current <see cref="ConnectionStringSettingsBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="alias"/> is <see langword="null"/>, empty, or whitespace.</exception>
        public ConnectionStringSettingsBuilder UseAlias(string alias)
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(alias), nameof(alias));
            _alias = alias;
            return this;
        }

        /// <summary>
        /// Appends to the current connection string value.
        /// </summary>
        /// <param name="connectionString">The connection string segment to append.</param>
        /// <returns>The current <see cref="ConnectionStringSettingsBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="connectionString"/> is <see langword="null"/>, empty, or whitespace.</exception>
        public ConnectionStringSettingsBuilder UseConnectionString(string connectionString)
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(connectionString), nameof(connectionString));
            _connectionString += connectionString;
            return this;
        }

        /// <summary>
        /// Uses the full name of <typeparamref name="TType"/> as the connection alias and assigns the supplied connection string.
        /// </summary>
        /// <typeparam name="TType">The type whose full name is used as the connection alias.</typeparam>
        /// <param name="connectionString">The connection string to assign.</param>
        /// <returns>The current <see cref="ConnectionStringSettingsBuilder"/> instance.</returns>
        public ConnectionStringSettingsBuilder UseConnectionString<TType>(string connectionString)
        {
            var alias = typeof(TType).FullName;
            return UseConnectionString(alias!, connectionString);
        }

        /// <summary>
        /// Sets both the alias and connection string in a single call.
        /// </summary>
        /// <param name="alias">The alias to assign.</param>
        /// <param name="connectionString">The connection string to assign.</param>
        /// <returns>The current <see cref="ConnectionStringSettingsBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="alias"/> or <paramref name="connectionString"/> is <see langword="null"/>, empty, or whitespace.</exception>
        public ConnectionStringSettingsBuilder UseConnectionString(string alias, string connectionString)
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(alias), nameof(alias));
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(connectionString), nameof(connectionString));

            _alias = alias!;
            _connectionString = connectionString;
            return this;
        }

        /// <summary>
        /// Builds a <see cref="ConnectionStringSetting"/> instance from the configured values.
        /// </summary>
        /// <returns>The built <see cref="ConnectionStringSetting"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the alias or connection string has not been configured.</exception>
        public ConnectionStringSetting Build()
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(_alias), nameof(_alias));
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(_connectionString), nameof(_connectionString));

            return new ConnectionStringSetting { Alias = _alias, ConnectionString = _connectionString };
        }
    }

}