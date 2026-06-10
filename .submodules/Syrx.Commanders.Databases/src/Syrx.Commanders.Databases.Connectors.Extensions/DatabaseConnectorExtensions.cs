namespace Syrx.Commanders.Databases.Connectors.Extensions
{
    /// <summary>
    /// Provides Syrx builder extensions for configuring the default database connector pipeline.
    /// </summary>
    public static class DatabaseConnectorExtensions
    {
        
        /// <summary>
        /// Registers the default connector, command reader, and commander services using the supplied provider and command settings delegates.
        /// </summary>
        /// <param name="builder">The builder used to configure Syrx.</param>
        /// <param name="providerFactory">The delegate that returns the <see cref="DbProviderFactory"/> used for connections.</param>
        /// <param name="settingsFactory">The delegate that configures command and connection settings.</param>
        /// <returns>The updated <see cref="SyrxBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="settingsFactory"/> is <see langword="null"/>.</exception>
        public static SyrxBuilder UseConnector(
            this SyrxBuilder builder,
            Func<DbProviderFactory> providerFactory,
            Action<CommanderSettingsBuilder> settingsFactory
            )
        {
            // installs a default connection with a given provider

            Throw(settingsFactory != null,
                () => new ArgumentNullException(nameof(settingsFactory),
                $"The {nameof(CommanderSettings)} delegate cannot be null."));

            var settings = CommanderSettingsBuilderExtensions.Build(settingsFactory!);
            builder.ServiceCollection
                .AddProvider(providerFactory)
                .AddSingleton<ICommanderSettings, CommanderSettings>(a => settings)
                .AddReader(ServiceLifetime.Singleton) 
                .AddDatabaseConnector<IDatabaseConnector, DatabaseConnector>(ServiceLifetime.Singleton)                 
                .AddDatabaseCommander(ServiceLifetime.Singleton);
            return builder;
        }

    }
}
