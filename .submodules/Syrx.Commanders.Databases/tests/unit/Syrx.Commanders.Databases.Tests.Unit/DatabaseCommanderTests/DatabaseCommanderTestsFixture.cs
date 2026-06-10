
namespace Syrx.Commanders.Databases.Tests.Unit.DatabaseCommanderTests
{
    public class DatabaseCommanderTestsFixture
    {
        private readonly IServiceProvider _provider;

        public DatabaseCommanderTestsFixture()
        {
            var installer = new Installer();
            _provider = installer.Install();            
        }

        protected internal ICommander<TRepository> GetCommander<TRepository>() => _provider.GetRequiredService<ICommander<TRepository>>();
    }
}
