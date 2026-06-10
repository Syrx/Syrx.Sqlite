namespace Syrx.Commanders.Databases.Connectors.Extensions
{
    /// <summary>
    /// Provides dependency injection registration helpers for database connector services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        // these exist tpo provide support for 
        // providers and connectors that have 
        // yet to be written... 

        /// <summary>
        /// Registers the delegate used to resolve the provider factory for connector-created connections.
        /// </summary>
        /// <param name="services">The service collection to update.</param>
        /// <param name="providerFactory">The delegate that returns the <see cref="DbProviderFactory"/> to use.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="providerFactory"/> is <see langword="null"/>.</exception>
        public static IServiceCollection AddProvider(
            this IServiceCollection services,
            Func<DbProviderFactory> providerFactory
            )
        {
            Throw(providerFactory != null,
                () => new ArgumentNullException(nameof(providerFactory),
                $"The {nameof(DbProviderFactory)} delegate cannot be null."));

            return services.TryAddToServiceCollection(
                typeof(Func<DbProviderFactory>),
                providerFactory!);
        }

            /// <summary>
            /// Registers a database connector implementation for the specified connector service contract.
            /// </summary>
            /// <typeparam name="TService">The connector service contract.</typeparam>
            /// <typeparam name="TImplementation">The concrete connector implementation.</typeparam>
            /// <param name="services">The service collection to update.</param>
            /// <param name="lifetime">The service lifetime used for the registration.</param>
            /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddDatabaseConnector<TService, TImplementation>(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient
            ) where TService : IDatabaseConnector where TImplementation : class, IDatabaseConnector
        {
            return services.TryAddToServiceCollection(
                typeof(TService),
                typeof(TImplementation),
                lifetime);
        }

    }
}
