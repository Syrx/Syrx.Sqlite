namespace Syrx.Commanders.Databases.Settings.Extensions
{
    /// <summary>
    /// Provides helper methods for building <see cref="NamespaceSetting"/> instances from fluent builder delegates.
    /// </summary>
    public static class NamespaceSettingBuilderExtensions
    {
        /// <summary>
        /// Creates a <see cref="NamespaceSetting"/> instance by executing the supplied builder delegate.
        /// </summary>
        /// <param name="factory">The delegate that configures a <see cref="NamespaceSettingBuilder"/> instance.</param>
        /// <returns>The built <see cref="NamespaceSetting"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <see langword="null"/>.</exception>
        public static NamespaceSetting Build(Action<NamespaceSettingBuilder> factory)
        {
            Throw<ArgumentNullException>(factory != null, nameof(factory));
            var builder = new NamespaceSettingBuilder();
            factory!(builder);
            return builder.Build();
        }
    }
}
