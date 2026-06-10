namespace Syrx.Commanders.Databases.Builders
{
    /// <summary>
    /// Builds a <see cref="Database"/> definition by collecting table metadata through a fluent API.
    /// </summary>
    public class DatabaseOptions
    {
        private string _name;
        private IDictionary<string, Table> _tables;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseOptions"/> class.
        /// </summary>
        public DatabaseOptions()
        {
            _name = string.Empty;
            _tables = new Dictionary<string, Table>();
        }

        /// <summary>
        /// Sets the database name that will be assigned to the built schema definition.
        /// </summary>
        /// <param name="name">The database name to assign.</param>
        /// <returns>The current <see cref="DatabaseOptions"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <see langword="null"/>, empty, or whitespace.</exception>
        public DatabaseOptions WithName(string name)
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(name), nameof(name));
            _name = name;
            return this;
        }

        /// <summary>
        /// Builds and adds a table definition using the supplied table builder delegate.
        /// </summary>
        /// <param name="factory">The delegate that configures the table definition.</param>
        /// <returns>The current <see cref="DatabaseOptions"/> instance.</returns>
        public DatabaseOptions AddTable(Action<TableOptions> factory)
        {
            var table = TableOptionsBuilderExtensions.Build(factory);
            return AddTable(table);
        }

        /// <summary>
        /// Adds an already constructed table definition to the database definition.
        /// </summary>
        /// <param name="table">The table definition to add.</param>
        /// <returns>The current <see cref="DatabaseOptions"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="table"/> is <see langword="null"/>.</exception>
        public DatabaseOptions AddTable(Table table)
        {
            Throw<ArgumentNullException>(table != null, nameof(table));
            _tables.Add(table!.Name, table);
            return this;
        }

        internal protected Database Build()
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(_name), nameof(_name));
            Throw<ArgumentNullException>(_tables != null, nameof(_tables));
            Throw<ArgumentOutOfRangeException>(_tables!.Any(), nameof(_tables));

            return new Database(_name, _tables!.Values.ToList());
        }
    }
}
