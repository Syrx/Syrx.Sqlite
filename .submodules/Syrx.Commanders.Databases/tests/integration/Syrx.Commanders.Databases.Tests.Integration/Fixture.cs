namespace Syrx.Commanders.Databases.Tests.Integration
{
    public abstract class Fixture
    {

        private IServiceProvider _provider;

        /// <summary>
        /// Allows us to hold assertion messages for the responses from 
        /// different RDBMS implementations.
        /// </summary>
        public AssertionMessages AssertionMessages { get; }

        protected Fixture()
        {
            AssertionMessages = new AssertionMessages();
                        
        }

        /// <summary>
        /// Called as a separate method as xUnit doesn't support 
        /// constructor arguments on fixture types. 
        /// </summary>
        /// <param name="provider"></param>
        public void Install(Func<IServiceProvider> provider)
        {
            _provider = provider();
            
        }

        /// <summary>
        /// Convenience method. 
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        public virtual ICommander<TRepository> ResolveCommander<TRepository>() => _provider.GetRequiredService<ICommander<TRepository>>();

    }
    
}
