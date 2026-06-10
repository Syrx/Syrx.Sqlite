namespace Syrx.Commanders.Databases.Settings
{
    /// <summary>
    /// Represents the command configuration for a repository namespace.
    /// </summary>
    public sealed record NamespaceSetting : INamespaceSetting<CommandSetting>
    {
        /// <summary>
        /// Gets the namespace that owns the configured repository types.
        /// </summary>
        public required string Namespace { get; init; }

        /// <summary>
        /// Gets the configured repository types within the namespace.
        /// </summary>
        public required List<TypeSetting> Types { get; init; }
        
        // Explicit interface implementation with both get and init
        IEnumerable<ITypeSetting<CommandSetting>> INamespaceSetting<CommandSetting>.Types 
        { 
            get => Types; 
            init => Types = value.OfType<TypeSetting>().ToList(); 
        }
    }
}
