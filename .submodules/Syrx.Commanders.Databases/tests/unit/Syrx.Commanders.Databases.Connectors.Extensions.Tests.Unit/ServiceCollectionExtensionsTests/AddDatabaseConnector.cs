
namespace Syrx.Commanders.Databases.Connectors.Extensions.Tests.Unit.ServiceCollectionExtensionsTests
{
    public class AddDatabaseConnector
    {
        private readonly IServiceCollection _services;

        public AddDatabaseConnector()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void RegistersCustomConnector()
        {
            _services.AddDatabaseConnector<IDatabaseConnector, AddDatabaseConnectorTestConnector>();
            var provider = _services.BuildServiceProvider();
            var result = provider.GetService<IDatabaseConnector>();
            NotNull(result);           
        }


        private class AddDatabaseConnectorTestConnector : IDatabaseConnector
        {
            public IDbConnection CreateConnection(CommandSetting options) 
                => throw new NotImplementedException("stub implementation for test only.");
        }

    }
}
