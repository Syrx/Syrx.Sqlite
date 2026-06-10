namespace Syrx.Sqlite.Tests.Integration
{
    [CollectionDefinition(nameof(SqliteFixtureCollection))]
    public class SqliteFixtureCollection : ICollectionFixture<Fixtures.SqliteFixture>
    {
    }
}
