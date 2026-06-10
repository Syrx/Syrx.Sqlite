
namespace Syrx.Commanders.Databases.Settings.Extensions.Xml
{
    /// <summary>
    /// Provides extension methods for registering Syrx XML settings files.
    /// </summary>
    public static class UseFileExtensions
    {
        /// <summary>
        /// Adds a trusted XML settings file to the supplied configuration builder.
        /// </summary>
        /// <param name="factory">The <see cref="SyrxBuilder"/> being configured.</param>
        /// <param name="fileName">The leaf XML file name to add to the configuration pipeline.</param>
        /// <param name="builder">The configuration builder that receives the XML file registration.</param>
        /// <returns>The same <see cref="SyrxBuilder"/> instance so configuration calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="builder"/> is <see langword="null"/>, or when <paramref name="fileName"/> is <see langword="null"/>, empty, or whitespace.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="fileName"/> is not an approved XML settings file name.
        /// </exception>
        public static SyrxBuilder UseFile(this SyrxBuilder factory, string fileName, IConfigurationBuilder builder)
        {
            Throw<ArgumentNullException>(builder != null, $"ConfigurationBuilder is null! Check bootstrap.");
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(fileName), nameof(fileName));
            Throw<ArgumentException>(IsTrustedXmlSettingsFileName(fileName),
                $"The filename '{fileName}' is not an approved XML settings file name.");

            builder?.AddXmlFile(fileName);

            return factory;
        }

        private static bool IsTrustedXmlSettingsFileName(string fileName)
        {
            var containsDirectorySeparator = fileName.Contains('/') || fileName.Contains('\\');
            var hasXmlExtension = string.Equals(Path.GetExtension(fileName), ".xml", StringComparison.OrdinalIgnoreCase);

            return !containsDirectorySeparator && hasXmlExtension;
        }
    }
}
