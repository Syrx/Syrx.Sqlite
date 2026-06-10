namespace Syrx.Commanders.Databases.Settings
{
    /// <summary>
    /// Concrete settings container used by the framework. Contains the
    /// namespace/type/method command mappings and optional connection strings.
    /// </summary>
    /// <remarks>
    /// This record is intended to be populated from configuration (JSON/XML) and
    /// injected into the components that need to resolve commands and connection strings.
    /// </remarks>
    public sealed record CommanderSettings : ICommanderSettings
    {
        /// <summary>
        /// The configured namespaces and their contained types and commands.
        /// </summary>
        public required List<NamespaceSetting> Namespaces { get; init; }

        /// <summary>
        /// Optional named connection strings referenced by <see cref="CommandSetting.ConnectionAlias"/>.
        /// </summary>
        public List<ConnectionStringSetting>? Connections { get; init; }
        
        // Explicit interface implementation for base ISettings<CommandSetting>
        IEnumerable<INamespaceSetting<CommandSetting>> ISettings<CommandSetting>.Namespaces => Namespaces;
    }
}
