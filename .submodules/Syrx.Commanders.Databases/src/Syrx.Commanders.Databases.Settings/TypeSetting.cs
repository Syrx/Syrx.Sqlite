namespace Syrx.Commanders.Databases.Settings
{
    /// <summary>
    /// Represents the command configuration for a single repository type.
    /// </summary>
    public sealed record TypeSetting : ITypeSetting<CommandSetting>
    {
        /// <summary>
        /// Gets the full type name used to match the repository.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Gets the command settings keyed by repository method name.
        /// </summary>
        public required ConcurrentDictionary<string, CommandSetting> Commands { get; init; }
        
        // Explicit interface implementation with both get and init
        IDictionary<string, CommandSetting> ITypeSetting<CommandSetting>.Commands 
        { 
            get => Commands; 
            init => Commands = value as ConcurrentDictionary<string, CommandSetting> ?? new ConcurrentDictionary<string, CommandSetting>(value); 
        }
    }
}
