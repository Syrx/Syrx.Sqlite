namespace Syrx.Sqlite.Tests.Integration.DatabaseCommanderTests
{
    [Collection(nameof(SqliteFixtureCollection))]
    public class SqliteExecuteAsync(Fixtures.SqliteFixture fixture) : ExecuteAsync(fixture)
    {
        [Theory(Skip = "Not supported by SQLite")]
        [MemberData(nameof(TransactionScopeOptions))]
        public override Task SupportsEnlistingInAmbientTransactions(TransactionScopeOption scopeOption)
        {
            return base.SupportsEnlistingInAmbientTransactions(scopeOption);
        }
    }
}
