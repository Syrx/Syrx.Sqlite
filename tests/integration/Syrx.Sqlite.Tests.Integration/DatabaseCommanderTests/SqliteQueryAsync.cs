namespace Syrx.Sqlite.Tests.Integration.DatabaseCommanderTests
{
    [Collection(nameof(SqliteFixtureCollection))]
    public class SqliteQueryAsync(Fixtures.SqliteFixture fixture) : QueryAsync(fixture) { }
}
