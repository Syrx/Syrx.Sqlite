namespace Syrx.Commanders.Databases.Settings.Extensions
{
    /// <summary>
    /// Provides helper methods for building <see cref="TypeSetting"/> instances from fluent builder delegates.
    /// </summary>
    public static class TypeSettingBuilderExtensions
    {
        /// <summary>
        /// Creates a <see cref="TypeSetting"/> instance by executing the supplied builder delegate.
        /// </summary>
        /// <typeparam name="TType">The repository type being configured.</typeparam>
        /// <param name="factory">The delegate that configures a <see cref="TypeSettingBuilder{TType}"/> instance.</param>
        /// <returns>The built <see cref="TypeSetting"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <see langword="null"/>.</exception>
        public static TypeSetting Build<TType>(Action<TypeSettingBuilder<TType>> factory)
        {
            Throw<ArgumentNullException>(factory != null, nameof(factory));
            var builder = new TypeSettingBuilder<TType>();
            factory!(builder);
            return builder.Build();
        }
    }
}
