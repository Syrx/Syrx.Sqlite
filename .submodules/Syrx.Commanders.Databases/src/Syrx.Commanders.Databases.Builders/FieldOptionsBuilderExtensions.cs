namespace Syrx.Commanders.Databases.Builders
{
    /// <summary>
    /// Provides helper methods for building <see cref="Field"/> instances from fluent builder delegates.
    /// </summary>
    public static class FieldOptionsBuilderExtensions
    {
        /// <summary>
        /// Creates a <see cref="Field"/> by executing the supplied field builder delegate.
        /// </summary>
        /// <param name="builder">The delegate that configures a <see cref="FieldOptions"/> instance.</param>
        /// <returns>The built <see cref="Field"/> instance.</returns>
        public static Field AddField(Action<FieldOptions> builder)
        {
            var options = new FieldOptions();
            builder.Invoke(options);
            return options.Build();
        }
    }
}
