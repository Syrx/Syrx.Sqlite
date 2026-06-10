namespace Syrx.Commanders.Databases.Connectors.Sqlite.Extensions.Tests.Unit.SqliteDatabaseConnectorExtensionsTests
{
    public class UseSqlite
    {
        private readonly IServiceCollection _services;

        public UseSqlite()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void Successful()
        {
            _services.UseSyrx(a => a
                .UseSqlite(b => b
                    .AddCommand(c => c
                        .ForType<UseSqlite>(d => d
                            .ForMethod(nameof(Successful), e => e.UseCommandText("test-command").UseConnectionAlias("test-alias"))))));

            var provider = _services.BuildServiceProvider();
            var connector = provider.GetService<IDatabaseConnector>();
            IsType<SqliteDatabaseConnector>(connector);
        }
    }
}
