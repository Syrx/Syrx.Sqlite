namespace Syrx.Commanders.Databases.Settings
{
    /// <summary>
    /// Describes configuration used by the commander and connector implementations.
    /// </summary>
    /// <remarks>
    /// Implementations are typically deserialized from JSON or XML configuration
    /// files and provide mappings from repository methods to SQL commands as well
    /// as named connection string aliases.
    /// </remarks>
    public interface ICommanderSettings : ISettings<CommandSetting>
    {
        /// <summary>
        /// Optional list of named connection string settings used by connectors.
        /// </summary>
        List<ConnectionStringSetting>? Connections { get; init; }

        /// <summary>
        /// List of namespace-level settings which contain types and command mappings.
        /// </summary>
        new List<NamespaceSetting> Namespaces { get; init; }
    }
}
