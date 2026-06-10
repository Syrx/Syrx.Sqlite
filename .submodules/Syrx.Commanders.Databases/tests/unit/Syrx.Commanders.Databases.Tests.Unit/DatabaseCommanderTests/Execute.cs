
namespace Syrx.Commanders.Databases.Tests.Unit.DatabaseCommanderTests
{
    public class Execute(DatabaseCommanderTestsFixture fixture) : IClassFixture<DatabaseCommanderTestsFixture>
    {
        private readonly ICommander<Execute> _commander = fixture.GetCommander<Execute>();


        [Fact]
        public void NullModelThrowsArgumentNullException()
        {
            var model = new { Name = Guid.NewGuid() };
            model = null;
            var result = Throws<ArgumentNullException>(() => _commander.Execute(model));
            result.ArgumentNull(nameof(model));
        }
    }

    public class Installer
    {
        public IServiceProvider Install(IServiceCollection services = null)
        {
            services = services ?? new ServiceCollection();
            var providerFactory = GetProviderFactory();

            services.UseSyrx(a => a
                .UseConnector(() => new Moq.Mock<DbProviderFactory>().Object, 
                    b => b
                .AddConnectionString("test", "test-connection-string")
                .AddCommand(c => c.ForType<Execute>(d => d
                    .ForMethod(nameof(Execute.NullModelThrowsArgumentNullException), e => e
                    .UseConnectionAlias("test")
                    .UseCommandText("select 1/0;")
                )))));


            return services.BuildServiceProvider();
        }

        public DbProviderFactory GetProviderFactory()
        {
            var mock = new Moq.Mock<DbProviderFactory>();
            return mock.Object;
        }
    }
}
