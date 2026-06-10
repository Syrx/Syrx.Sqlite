namespace Syrx.Commanders.Databases.Connectors.Sqlite.Tests.Unit.SqliteDatabaseConnectorTests
{
    public class Constructor
    {
        private readonly CommanderSettings _settings;

        public Constructor()
        {
            _settings = CommanderSettingsBuilderExtensions.Build(
                a => a.AddCommand(
                    b => b.ForType<Constructor>(
                        c => c.ForMethod(nameof(Successfully),
                        d => d.UseConnectionAlias("test-alias")
                        .UseCommandText("test-command-text")))));
        }

        [Fact]
        public void NullSettingsThrowsArgumentNullException()
        {
            var result = Throws<ArgumentNullException>(() => new SqliteDatabaseConnector(null));
            result.ArgumentNull("settings");
        }

        [Fact]
        public void Successfully()
        {
            var result = new SqliteDatabaseConnector(_settings);
            NotNull(result);
        }
    }
}
