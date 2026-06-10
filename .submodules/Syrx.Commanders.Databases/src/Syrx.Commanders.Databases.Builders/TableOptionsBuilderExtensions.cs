namespace Syrx.Commanders.Databases.Builders
{
    /// <summary>
    /// Provides helper methods for building <see cref="Table"/> instances from fluent builder delegates.
    /// </summary>
    public static class TableOptionsBuilderExtensions
    {
        /// <summary>
        /// Creates a <see cref="Table"/> by executing the supplied builder delegate.
        /// </summary>
        /// <param name="builder">The delegate that configures a <see cref="TableOptions"/> instance.</param>
        /// <returns>The built <see cref="Table"/> instance.</returns>
        public static Table Build(Action<TableOptions> builder)
        {
            var options = new TableOptions();
            builder(options);
            return options.Build();
        }
    }
}
