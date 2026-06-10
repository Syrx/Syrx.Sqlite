namespace Syrx.Commanders.Databases.Builders
{
    /// <summary>
    /// Represents a table definition within a database schema model.
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Gets the schema that contains the table.
        /// </summary>
        public string Schema { get; }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the fields defined for the table.
        /// </summary>
        public IEnumerable<Field> Fields { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        /// <param name="name">The table name.</param>
        /// <param name="fields">The fields defined for the table.</param>
        /// <param name="schema">The schema that contains the table. Defaults to <c>dbo</c> when omitted or blank.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <see langword="null"/>, empty, or whitespace, or when <paramref name="fields"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="fields"/> does not contain any entries.</exception>
        public Table(string name, IEnumerable<Field> fields, string schema = "dbo")
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(name), nameof(name));
            Throw<ArgumentNullException>(fields != null, nameof(fields));
            Throw<ArgumentOutOfRangeException>(fields!.Any(), nameof(fields));

            Name = name;
            Fields = fields!;
            Schema = string.IsNullOrWhiteSpace(schema) ? "dbo" : schema;
        }
    }
}
