namespace Syrx.Sqlite.Tests.Integration.DatabaseCommanderTests
{
    [Collection(nameof(SqliteFixtureCollection))]
    public class SqliteDispose(Fixtures.SqliteFixture fixture) : Dispose(fixture) { }
}
