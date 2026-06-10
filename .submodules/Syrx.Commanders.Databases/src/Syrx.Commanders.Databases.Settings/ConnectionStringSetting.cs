namespace Syrx.Commanders.Databases.Settings
{
    /// <summary>
    /// Represents a named connection string entry used by the database connector.
    /// </summary>
    public sealed record ConnectionStringSetting
    {
        /// <summary>
        /// Gets the raw connection string value.
        /// </summary>
        public required string ConnectionString { get; init; }

        /// <summary>
        /// Gets the alias used to reference the connection string from command settings.
        /// </summary>
        public required string Alias { get; init; }
    }
}
