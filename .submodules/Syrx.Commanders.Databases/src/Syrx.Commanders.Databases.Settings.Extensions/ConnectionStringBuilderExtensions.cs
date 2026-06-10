namespace Syrx.Commanders.Databases.Settings.Extensions
{
    /// <summary>
    /// Provides helper methods for building <see cref="ConnectionStringSetting"/> instances from fluent builder delegates.
    /// </summary>
    public static class ConnectionStringBuilderExtensions
    {
        /// <summary>
        /// Creates a <see cref="ConnectionStringSetting"/> instance by executing the supplied builder delegate.
        /// </summary>
        /// <param name="factory">The delegate that configures a <see cref="ConnectionStringSettingsBuilder"/> instance.</param>
        /// <returns>The built <see cref="ConnectionStringSetting"/> instance.</returns>
        public static ConnectionStringSetting Build(Action<ConnectionStringSettingsBuilder> factory)
        {
            var builder = new ConnectionStringSettingsBuilder();
            factory(builder);
            return builder.Build();
        }
    }

}