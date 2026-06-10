
namespace Syrx.Commanders.Databases.Settings.Readers.Extensions
{
    /// <summary>
    /// Provides dependency injection registration helpers for command reader services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the default <see cref="IDatabaseCommandReader"/> implementation.
        /// </summary>
        /// <param name="services">The service collection to update.</param>
        /// <param name="lifetime">The service lifetime used for the registration.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddReader(
           this IServiceCollection services,
           ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            return services.TryAddToServiceCollection(
                typeof(IDatabaseCommandReader),
                typeof(DatabaseCommandReader),
                lifetime);
        }
    }
}
