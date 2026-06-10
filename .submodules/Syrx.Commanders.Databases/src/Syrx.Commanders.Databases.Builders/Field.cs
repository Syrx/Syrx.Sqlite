namespace Syrx.Commanders.Databases.Builders
{
    /// <summary>
    /// Represents a field definition within a table schema model.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Gets the field name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the SQL data type of the field.
        /// </summary>
        public SqlDbType Type { get; }

        /// <summary>
        /// Gets a value indicating whether the field permits <see langword="null"/> values.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// Gets the optional field width or length when the type supports it.
        /// </summary>
        public int? Width { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="type">The SQL data type.</param>
        /// <param name="width">The optional width or length when applicable to the SQL data type.</param>
        /// <param name="isNullable"><see langword="true"/> if the field permits <see langword="null"/> values; otherwise, <see langword="false"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <see langword="null"/>, empty, or whitespace.</exception>
        public Field(string name, SqlDbType type, int? width = null, bool isNullable = true)
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(name), nameof(name));

            Name = name;
            Type = type;
            Width = width;
            IsNullable = isNullable;
        }
    }
}
