namespace Syrx.Commanders.Databases.Builders
{
    /// <summary>
    /// Provides helper methods for building <see cref="Database"/> instances from fluent builder delegates.
    /// </summary>
    public static class DatabaseOptionsBuilderExtensions
    {
        /// <summary>
        /// Creates a <see cref="Database"/> by executing the supplied builder delegate.
        /// </summary>
        /// <param name="builder">The delegate that configures a <see cref="DatabaseOptions"/> instance.</param>
        /// <returns>The built <see cref="Database"/> instance.</returns>
        public static Database Build(Action<DatabaseOptions> builder)
        {
            var options = new DatabaseOptions();
            builder(options);
            return options.Build();
        }
    }
}
