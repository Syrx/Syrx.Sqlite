namespace Syrx.Sqlite.Tests.Integration.DatabaseCommanderTests
{
    [Collection(nameof(SqliteFixtureCollection))]
    public class SqliteQuery(Fixtures.SqliteFixture fixture) : Query(fixture) { }
}
