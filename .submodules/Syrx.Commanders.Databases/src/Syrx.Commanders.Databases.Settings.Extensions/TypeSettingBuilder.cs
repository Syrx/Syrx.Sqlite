namespace Syrx.Commanders.Databases.Settings.Extensions
{
    /// <summary>
    /// Builds a <see cref="TypeSetting"/> instance by collecting command mappings for a repository type.
    /// </summary>
    /// <typeparam name="TType">The repository type being configured.</typeparam>
    public class TypeSettingBuilder<TType>
    {
        private string _name;
        private ConcurrentDictionary<string, CommandSetting> _commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSettingBuilder{TType}"/> class.
        /// </summary>
        public TypeSettingBuilder()
        {
            var type = typeof(TType);
            _name = type!.FullName!;
            _commands = new ConcurrentDictionary<string, CommandSetting>();
        }

        /// <summary>
        /// Adds command configuration for a repository method.
        /// </summary>
        /// <param name="method">The repository method name.</param>
        /// <param name="builder">The delegate that configures the command setting.</param>
        /// <returns>The current <see cref="TypeSettingBuilder{TType}"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="method"/> is <see langword="null"/>, empty, or whitespace, or when <paramref name="builder"/> is <see langword="null"/>.</exception>
        public TypeSettingBuilder<TType> ForMethod(string method, Action<CommandSettingBuilder> builder)
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(method), nameof(method));
            Throw<ArgumentNullException>(builder != null, nameof(builder));

            var settings = CommandSettingBuilderExtensions.Build(builder!);

            if (_commands.TryGetValue(method, out _))
            {
                return this;
            }

            _commands.TryAdd(method, settings!);
            return this;
        }

        protected internal TypeSetting Build()
        {
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(_name), nameof(_name));
            Throw<ArgumentException>(_commands.Count != 0, $"At least 1 command is expected to be set for type '{_name}'");

            return new TypeSetting { Commands = _commands, Name = _name };
        }
    }
}
