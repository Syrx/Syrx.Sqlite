
namespace Syrx.Commanders.Databases.Connectors.Extensions.Tests.Unit.ServiceCollectionExtensionsTests
{
    public class AddProvider
    {
        private readonly IServiceCollection _services;

        public AddProvider()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void NullProviderThrowsArgumentNullException()
        {
            var result = Throws<ArgumentNullException>(() => _services.AddProvider(null));
            result.HasMessage("The DbProviderFactory delegate cannot be null. (Parameter 'providerFactory')");
        }

        [Fact]
        public void Successfully()
        {
            var connection = new Mock<DbConnection>();
            var dbProviderFactory = new Mock<DbProviderFactory>();
            dbProviderFactory.Setup(x => x.CreateConnection()).Returns(connection.Object);

            Func<DbProviderFactory> factory = () => dbProviderFactory.Object;
            var result = _services.AddProvider(factory);

            var provider = _services.BuildServiceProvider();
            var resolved = provider.GetService<Func<DbProviderFactory>>();
            NotNull(resolved);
            Same(factory, resolved);
        }
    }    
}
