namespace Syrx.Commanders.Databases.Settings.Extensions
{
    /// <summary>
    /// Provides helper methods for building <see cref="CommanderSettings"/> instances from fluent builder delegates.
    /// </summary>
    public static class CommanderSettingsBuilderExtensions
    {
        /// <summary>
        /// Creates a <see cref="CommanderSettings"/> instance by executing the supplied builder delegate.
        /// </summary>
        /// <param name="factory">The delegate that configures a <see cref="CommanderSettingsBuilder"/> instance.</param>
        /// <returns>The built <see cref="CommanderSettings"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <see langword="null"/>.</exception>
        public static CommanderSettings Build(Action<CommanderSettingsBuilder> factory)
        {
            Throw<ArgumentNullException>(factory != null, nameof(factory));
            var builder = new CommanderSettingsBuilder();
            factory!(builder);
            var result = builder.Build();
            return result;
        }
    }

}
