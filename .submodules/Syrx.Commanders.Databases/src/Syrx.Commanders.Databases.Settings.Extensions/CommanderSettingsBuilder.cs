namespace Syrx.Commanders.Databases.Settings.Extensions
{
    /// <summary>
    /// Builds a <see cref="CommanderSettings"/> instance by collecting connection string and command mappings.
    /// </summary>
    public class CommanderSettingsBuilder
    {
        private ConcurrentDictionary<string, ConnectionStringSetting> _connectionStrings;
        private ConcurrentDictionary<string, NamespaceSetting> _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommanderSettingsBuilder"/> class.
        /// </summary>
        public CommanderSettingsBuilder()
        {
            _connectionStrings = new ConcurrentDictionary<string, ConnectionStringSetting>();
            _settings = new ConcurrentDictionary<string, NamespaceSetting>();
        }

        /// <summary>
        /// Adds a named connection string using the supplied alias and connection string values.
        /// </summary>
        /// <param name="alias">The alias used to reference the connection string.</param>
        /// <param name="connectionString">The connection string value.</param>
        /// <returns>The current <see cref="CommanderSettingsBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="alias"/> or <paramref name="connectionString"/> is <see langword="null"/>, empty, or whitespace.</exception>
        public CommanderSettingsBuilder AddConnectionString(string alias, string connectionString)
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(alias), nameof(alias));
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(connectionString), nameof(connectionString));
            Action<ConnectionStringSettingsBuilder> builder = (a) => a.UseAlias(alias).UseConnectionString(connectionString);
            return AddConnectionString(builder);
        }

        /// <summary>
        /// Adds a named connection string using the supplied builder delegate.
        /// </summary>
        /// <param name="builder">The delegate that configures a <see cref="ConnectionStringSettingsBuilder"/> instance.</param>
        /// <returns>The current <see cref="CommanderSettingsBuilder"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the alias already exists with a different connection string value.</exception>
        public CommanderSettingsBuilder AddConnectionString(Action<ConnectionStringSettingsBuilder> builder)
        {
            var settings = ConnectionStringBuilderExtensions.Build(builder);

            var existing = _connectionStrings.GetOrAdd(settings.Alias, settings);
            if (existing.ConnectionString != settings.ConnectionString)
            {
                Throw<ArgumentException>(existing.ConnectionString == settings.ConnectionString, $"The alias '{settings.Alias}' is already assigned to a different connection string.");
            }

            return this;
        }

        /// <summary>
        /// Adds a command namespace definition using the supplied builder delegate.
        /// </summary>
        /// <param name="builder">The delegate that configures a <see cref="NamespaceSettingBuilder"/> instance.</param>
        /// <returns>The current <see cref="CommanderSettingsBuilder"/> instance.</returns>
        public CommanderSettingsBuilder AddCommand(Action<NamespaceSettingBuilder> builder)
        {
            var options = NamespaceSettingBuilderExtensions.Build(builder);
            return AddCommand(options);
        }

        /// <summary>
        /// Adds a command namespace definition to the builder.
        /// </summary>
        /// <param name="options">The namespace definition to add.</param>
        /// <returns>The current <see cref="CommanderSettingsBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        public CommanderSettingsBuilder AddCommand(NamespaceSetting options)
        {
            Throw<ArgumentNullException>(options != null, nameof(options));
            Evaluate(options!);
            return this;
        }

        private void Evaluate(NamespaceSetting option)
        {
            // pretty sure this can be done more elegantly. 
            if (_settings.TryGetValue(option.Namespace, out var ns))
            {
                // for this namespace, we need to check that the 
                // types collection doesn't already contain the 
                // type being passed by 'option'. 

                // for the current approach we need to 
                // evaluate each of the types in 'option.Types'
                // to see if they should be assigned to that type. 

                foreach (var type in option.Types)
                {
                    // evaluate whether we are already hosting this type. 
                    var entry = ns.Types.SingleOrDefault(x => x.Name == type.Name);

                    // if that's true, add these type.Commands to that that type. 
                    if (entry != null)
                    {
                        foreach (var (method, command) in type.Commands)
                        {
                            entry.Commands.TryAdd(method, command);
                        }
                        return;
                    }

                    // as we're already in the same namespace, add
                    // to that one instead. 
                    ns.Types.AddRange(option.Types);
                    return;
                }
            }

            _settings.TryAdd(option.Namespace, option);
        }

        private IEnumerable<NamespaceSetting> ValidateNamespaceCollections(IEnumerable<NamespaceSetting> collection)
        {
            Throw<ArgumentException>(collection.Any(), "Collection should have at least 1 namespace setting. Use the AddCommand method to add a new entry to the collection.");
            return collection;
        }

        protected internal CommanderSettings Build()
        {
            // the connections collections can be empty as we 
            // might want tp pull these from a different source/secrets. 
            var connections = _connectionStrings.Values.ToList();
            var namespaces = ValidateNamespaceCollections(_settings.Values);


            return new CommanderSettings { Namespaces = namespaces.ToList(), Connections = connections };
        }
    }

}
