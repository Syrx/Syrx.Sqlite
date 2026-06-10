// ========================================================================================================================================================
// author      : david sexton (@sextondjc | sextondjc.com)
// modified    : 2020.06.21 (20:51)
// site        : https://www.github.com/syrx
// ========================================================================================================================================================

namespace Syrx.Commanders.Databases.Extensions
{
    /// <summary>
    /// Provides dependency injection registration helpers for database commander services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="ICommander{TRepository}"/> with the default <see cref="DatabaseCommander{TRepository}"/> implementation.
        /// </summary>
        /// <param name="services">The service collection to update.</param>
        /// <param name="lifetime">The service lifetime used for the registration.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddDatabaseCommander(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            return services
                .TryAddToServiceCollection(typeof(ICommander<>), typeof(DatabaseCommander<>), lifetime);

        }
    }
}