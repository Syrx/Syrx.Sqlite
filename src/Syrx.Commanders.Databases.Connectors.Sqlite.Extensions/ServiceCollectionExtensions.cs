namespace Syrx.Commanders.Databases.Connectors.Sqlite.Extensions
{
    /// <summary>
    /// Contains service registration helpers for the SQLite connector implementation.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddSqlite(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            return services.TryAddToServiceCollection(
                typeof(IDatabaseConnector),
                typeof(SqliteDatabaseConnector),
                lifetime);
        }
    }
}
