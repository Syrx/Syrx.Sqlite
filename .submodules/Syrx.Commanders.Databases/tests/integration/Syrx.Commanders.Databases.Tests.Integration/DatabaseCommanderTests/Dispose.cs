namespace Syrx.Commanders.Databases.Tests.Integration.DatabaseCommanderTests
{
    public abstract class Dispose(Fixture fixture)
    {

        [Fact]
        public void Successfully()
        {
            // there's nothing to actually dispose of so... 
            var commander = fixture.ResolveCommander<Dispose>();
            commander.Dispose();
        }
    }

}
