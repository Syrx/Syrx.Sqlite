//  ============================================================================================================================= 
//  author       : david sexton (@sextondjc | sextondjc.com)
//  date         : 2017.10.15 (17:58)
//  licence      : This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
//  =============================================================================================================================

namespace Syrx.Commanders.Databases.Settings.Readers
{
    /// <summary>
    /// Reads configured command settings and resolves them by repository type and method name.
    /// </summary>
    /// <remarks>
    /// Command settings are indexed eagerly during construction to avoid repeated traversal of the
    /// hierarchical settings model during command resolution.
    /// </remarks>
    public class DatabaseCommandReader : IDatabaseCommandReader
    {
        private readonly Dictionary<string, CommandSetting> _commandLookup = new(StringComparer.Ordinal);
                
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseCommandReader"/> class.
        /// </summary>
        /// <param name="settings">The commander settings containing namespaces, types, and commands to index.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="settings"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when duplicate command keys are found while indexing the settings.</exception>
        public DatabaseCommandReader(ICommanderSettings settings)
        {
            Throw<ArgumentNullException>(settings != null, "{0}. No settings were passed to DatabaseCommandReader.", nameof(settings));

            foreach (var @namespace in settings!.Namespaces ?? Enumerable.Empty<NamespaceSetting>())
            {
                foreach (var type in @namespace.Types ?? Enumerable.Empty<TypeSetting>())
                {
                    foreach (var command in type.Commands ?? Enumerable.Empty<KeyValuePair<string, CommandSetting>>())
                    {
                        var fullKey = $"{type.Name}.{command.Key}";
                        Throw<ArgumentException>(
                            _commandLookup.TryAdd(fullKey, command.Value),
                            $"Duplicate command setting key '{fullKey}' was found while indexing command settings.");
                    }
                }
            }
        }
                
        /// <summary>
        /// Gets the configured command setting for the specified repository type and method key.
        /// </summary>
        /// <param name="type">The repository type used as part of the command lookup key.</param>
        /// <param name="key">The method or command name associated with the repository type.</param>
        /// <returns>The resolved <see cref="CommandSetting"/> for the requested repository method.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="type"/> is <see langword="null"/>, or when <paramref name="key"/> is <see langword="null"/>, empty, or whitespace.
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// Thrown when no command setting exists for the combination of <paramref name="type"/> and <paramref name="key"/>.
        /// </exception>
        public CommandSetting GetCommand(Type type, string key)
        {
            Throw<ArgumentNullException>(type != null, nameof(type));
            Throw<ArgumentNullException>(!string.IsNullOrWhiteSpace(key), nameof(key));

            var lookupKey = $"{type!.FullName}.{key}";
            _commandLookup.TryGetValue(lookupKey, out var result);

            Throw<NullReferenceException>(result != null, ErrorMessages.NoCommandSetting, key, type!.FullName!);

            return result!;
        }

        private static class ErrorMessages
        {
            internal const string NoCommandSetting =
                    @"The command setting '{0}' has no entry for the type setting '{1}'. Please add a command setting entry to the type setting.";
        }
    }
}