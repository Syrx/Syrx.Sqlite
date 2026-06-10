namespace Syrx.Commanders.Databases.Settings.Extensions
{
    /// <summary>
    /// Builds a <see cref="NamespaceSetting"/> instance by collecting repository type command mappings.
    /// </summary>
    public class NamespaceSettingBuilder
    {
        private string _namespace;
        private ConcurrentDictionary<string, TypeSetting> _types;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceSettingBuilder"/> class.
        /// </summary>
        public NamespaceSettingBuilder()
        {
            _namespace = string.Empty;
            _types = new ConcurrentDictionary<string, TypeSetting>();
        }

        /// <summary>
        /// Adds command configuration for the specified repository type.
        /// </summary>
        /// <typeparam name="TType">The repository type to configure.</typeparam>
        /// <param name="builder">The delegate that configures the type-specific command mappings.</param>
        /// <returns>The current <see cref="NamespaceSettingBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the repository type does not have a namespace or when <paramref name="builder"/> is <see langword="null"/>.</exception>
        public NamespaceSettingBuilder ForType<TType>(Action<TypeSettingBuilder<TType>> builder)
        {
            var type = typeof(TType);
            var @namespace = type.Namespace;
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(@namespace), nameof(type.Namespace));
            Throw<ArgumentNullException>(builder != null, nameof(builder));

            var options = TypeSettingBuilderExtensions.Build(builder!);
            Throw<ArgumentNullException>(options != null, $"Error in adding command using ForType<> on '{type.FullName}'");

            _namespace = @namespace!;
            Evaluate(options!);
            return this;
        }

        /// <summary>
        /// Merges the supplied type setting into the namespace definition.
        /// </summary>
        /// <param name="option">The type setting to merge.</param>
        public void Evaluate(TypeSetting option)
        {
            // pretty sure this can be done more elegantly. 
            if (_types.TryGetValue(option.Name, out var type))
            {
                foreach (var (method, command) in option.Commands)
                {
                    type.Commands.TryAdd(method, command);
                }
                return;
            }

            _types.TryAdd(option.Name, option);
        }


        protected internal NamespaceSetting Build()
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(_namespace), nameof(_namespace));
            Throw<ArgumentException>(_types.Any(), $"At least 1 type was expected to be added to {_namespace}");

            return new NamespaceSetting
            {
                Namespace = _namespace,
                Types = _types.Values.ToList()
            };
        }
    }
}
