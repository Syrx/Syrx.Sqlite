namespace Syrx.Commanders.Databases.Connectors.Sqlite.Tests.Unit.SqliteDatabaseConnectorTests
{
    public class CreateConnection
    {
        private readonly CommanderSettings _settings;
        private readonly IDatabaseConnector _connector;

        public CreateConnection()
        {
            _settings = CommanderSettingsBuilderExtensions.Build(
                a => a.AddConnectionString("test.alias", "Data Source=:memory:;")
                .AddCommand(
                    b => b.ForType<CreateConnection>(
                    c => c.ForMethod(nameof(Successfully),
                    d => d.UseConnectionAlias("test.alias")
                          .UseCommandText("select 'readers.test.settings'")))));

            _connector = new SqliteDatabaseConnector(_settings);
        }

        [Fact]
        public void Successfully()
        {
            var setting = _settings.Namespaces.First().Types.First().Commands.First().Value;
            var result = _connector.CreateConnection(setting);
            Equal(ConnectionState.Closed, result.State);
        }
    }
}
