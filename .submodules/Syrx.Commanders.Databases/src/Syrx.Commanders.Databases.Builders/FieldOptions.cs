namespace Syrx.Commanders.Databases.Builders
{
    /// <summary>
    /// Builds a <see cref="Field"/> definition by collecting column metadata through a fluent API.
    /// </summary>
    public class FieldOptions
    {
        private string? _name;
        private SqlDbType _type;
        private int? _width;
        private bool _nullable;
        private bool _identity;

        /// <summary>
        /// Sets the field name.
        /// </summary>
        /// <param name="name">The field name to assign.</param>
        /// <returns>The current <see cref="FieldOptions"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <see langword="null"/>, empty, or whitespace.</exception>
        public FieldOptions WithName(string name)
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(name), nameof(name));
            _name = name;
            return this;
        }

        /// <summary>
        /// Sets the SQL data type for the field.
        /// </summary>
        /// <param name="type">The SQL data type.</param>
        /// <returns>The current <see cref="FieldOptions"/> instance.</returns>
        public FieldOptions WithDataType(SqlDbType type)
        {
            _type = type;
            return this;
        }

        /// <summary>
        /// Sets the optional width or length for the field.
        /// </summary>
        /// <param name="width">The width or length to assign, or <see langword="null"/> when not applicable.</param>
        /// <returns>The current <see cref="FieldOptions"/> instance.</returns>
        public FieldOptions HasWidth(int? width)
        {

            _width = width;
            return this;
        }

        /// <summary>
        /// Marks whether the field permits <see langword="null"/> values.
        /// </summary>
        /// <param name="nullable"><see langword="true"/> to allow <see langword="null"/> values; otherwise, <see langword="false"/>.</param>
        /// <returns>The current <see cref="FieldOptions"/> instance.</returns>
        public FieldOptions IsNullable(bool nullable = true)
        {
            _nullable = nullable;
            return this;
        }

        /// <summary>
        /// Marks whether the field should be treated as an identity column.
        /// </summary>
        /// <param name="identity"><see langword="true"/> to mark the field as identity; otherwise, <see langword="false"/>.</param>
        /// <returns>The current <see cref="FieldOptions"/> instance.</returns>
        public FieldOptions IsIdentity(bool identity = false)
        {
            _identity = identity;
            return this;
        }

        internal protected Field Build()
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(_name), nameof(_name));
            return new Field(_name!, _type, _width, _nullable);
        }
    }
}
