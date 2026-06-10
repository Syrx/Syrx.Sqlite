namespace Syrx.Commanders.Databases.Builders
{
    /// <summary>
    /// Represents a database schema definition containing tables and their structure.
    /// Used for code generation and schema analysis scenarios.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the collection of tables contained within this database.
        /// </summary>
        public IEnumerable<Table> Tables { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="name">The name of the database. Cannot be null or whitespace.</param>
        /// <param name="tables">The collection of tables. Cannot be null and must contain at least one table.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or whitespace, or <paramref name="tables"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="tables"/> is empty.</exception>
        public Database(
            string name,
            IEnumerable<Table> tables)
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(name), nameof(name));
            Throw<ArgumentNullException>(tables != null, nameof(tables));
            Throw<ArgumentOutOfRangeException>(tables!.Any(), nameof(tables));

            Name = name!;
            Tables = tables!;
        }
    }
}
