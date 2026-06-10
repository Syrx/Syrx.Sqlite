namespace Syrx.Commanders.Databases.Settings.Extensions.Tests.Unit.CommanderSettingsBuilderTests
{
    public class AddConnectionString
    {
        private CommanderSettingsBuilder _builder;
        private const string ConnectionString = "test-connection-string";
        private const string Alias = "test-alias";


        public AddConnectionString()
        {
            _builder = new CommanderSettingsBuilder();
        }

        [Theory]
        [MemberData(nameof(Generators.NullEmptyWhiteSpace), MemberType = typeof(Generators))]
        public void NullEmptyWhitespaceAliasThrowsArgumentNullException(string alias)
        {
            var result = Throws<ArgumentNullException>(() => _builder.AddConnectionString(alias, ConnectionString));
            result.ArgumentNull(nameof(alias));
        }

        [Fact]
        public void AddSingle()
        {
            var result = CommanderSettingsBuilderExtensions.Build(a => a
                .AddConnectionString(Alias, ConnectionString)
                .AddTestCommand()
                );

            Single(result.Connections);
            Equal(Alias, result.Connections.Single().Alias);
            Equal(ConnectionString, result.Connections.Single().ConnectionString);
        }

        [Fact]
        public void DuplicateConnectionStringsReturnsOnlyOne()
        {
            var result = CommanderSettingsBuilderExtensions.Build(a => a
                .AddConnectionString(Alias, ConnectionString)
                .AddConnectionString(Alias, ConnectionString)
                .AddTestCommand()
                );

            Single(result.Connections);
            Equal(Alias, result.Connections.Single().Alias);
            Equal(ConnectionString, result.Connections.Single().ConnectionString);
        }

        [Fact]
        public void DifferentConnectionStringsSameAliasThrowsArgumentExceptionWithoutLeakingSecrets()
        {
            const string otherConnectionString = "different-test-connection-string";

            var result = Throws<ArgumentException>(() => CommanderSettingsBuilderExtensions.Build(a => a
                .AddConnectionString(Alias, ConnectionString)
                .AddConnectionString(Alias, otherConnectionString)
                .AddTestCommand()));

            Contains(Alias, result.Message);
            DoesNotContain(ConnectionString, result.Message);
            DoesNotContain(otherConnectionString, result.Message);
        }
    }

    
}
