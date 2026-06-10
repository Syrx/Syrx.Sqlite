namespace Syrx.Commanders.Databases.Connectors.Sqlite.Extensions
{
    /// <summary>
    /// Adds SQLite-specific registration helpers to the Syrx builder pipeline.
    /// </summary>
    public static class SqliteConnectorExtensions
    {
        /// <summary>
        /// Registers Syrx SQLite settings, readers, connectors, and database commander services.
        /// </summary>
        public static SyrxBuilder UseSqlite(
            this SyrxBuilder builder,
            Action<CommanderSettingsBuilder> factory,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var options = CommanderSettingsBuilderExtensions.Build(factory);
            builder.ServiceCollection
                .AddSingleton<ICommanderSettings, CommanderSettings>(a => options)
                .AddReader(lifetime)
                .AddSqlite(lifetime)
                .AddDatabaseCommander(lifetime);

            return builder;
        }
    }
}
