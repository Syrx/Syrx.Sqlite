namespace Syrx.Commanders.Databases.Builders
{
    /// <summary>
    /// Builds a <see cref="Table"/> definition by collecting schema and field metadata through a fluent API.
    /// </summary>
    public class TableOptions
    {
        private string _name;
        private string _schema;
        private IDictionary<string, Field> _fields;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableOptions"/> class.
        /// </summary>
        public TableOptions()
        {
            _fields = new Dictionary<string, Field>();
            _name = string.Empty;
            _schema = string.Empty;
        }

        /// <summary>
        /// Sets the table name.
        /// </summary>
        /// <param name="name">The table name to assign.</param>
        /// <returns>The current <see cref="TableOptions"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <see langword="null"/>, empty, or whitespace.</exception>
        public TableOptions WithName(string name)
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(name), nameof(name));
            _name = name;
            return this;
        }

        /// <summary>
        /// Sets the schema name for the table.
        /// </summary>
        /// <param name="schema">The schema name to assign. Defaults to <c>dbo</c>.</param>
        /// <returns>The current <see cref="TableOptions"/> instance.</returns>
        public TableOptions WithSchema(string schema = "dbo")
        {
            _schema = schema;
            return this;
        }

        /// <summary>
        /// Builds and adds a field definition using the supplied field builder delegate.
        /// </summary>
        /// <param name="builder">The delegate that configures the field definition.</param>
        /// <returns>The current <see cref="TableOptions"/> instance.</returns>
        public TableOptions AddField(Action<FieldOptions> builder)
        {
            var field = FieldOptionsBuilderExtensions.AddField(builder);
            return AddField(field);
        }

        /// <summary>
        /// Adds an already constructed field definition to the table definition.
        /// </summary>
        /// <param name="field">The field definition to add.</param>
        /// <returns>The current <see cref="TableOptions"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="field"/> is <see langword="null"/>.</exception>
        public TableOptions AddField(Field field)
        {
            Throw<ArgumentNullException>(field != null, nameof(field));
            _fields.Add(field!.Name, field);
            return this;
        }

        internal protected Table Build()
        {
            // validate before returning. 
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(_name), nameof(Table.Name));
            Throw<ArgumentNullException>(_fields != null, nameof(Table.Fields));
            Throw<ArgumentOutOfRangeException>(_fields!.Any(), nameof(Table.Fields));

            return new Table(_name, _fields!.Values, _schema);
        }
    }
}
