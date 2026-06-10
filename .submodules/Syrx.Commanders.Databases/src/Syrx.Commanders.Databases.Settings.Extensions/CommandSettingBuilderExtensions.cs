namespace Syrx.Commanders.Databases.Settings.Extensions
{
    /// <summary>
    /// Provides helper methods for building <see cref="CommandSetting"/> instances from fluent builder delegates.
    /// </summary>
    public static class CommandSettingBuilderExtensions
    {
        /// <summary>
        /// Creates a <see cref="CommandSetting"/> instance by executing the supplied builder delegate.
        /// </summary>
        /// <param name="factory">The delegate that configures a <see cref="CommandSettingBuilder"/> instance.</param>
        /// <returns>The built <see cref="CommandSetting"/> instance.</returns>
        public static CommandSetting Build(Action<CommandSettingBuilder> factory)
        {
            var builder = new CommandSettingBuilder();
            factory(builder);
            return builder.Build();
        }
    }

}