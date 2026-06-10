namespace Syrx.Sqlite.Tests.Integration.DatabaseCommanderTests
{
    [Collection(nameof(SqliteFixtureCollection))]
    public class SqliteExecute(Fixtures.SqliteFixture fixture) : Execute(fixture)
    {
        [Theory(Skip = "Not supported by SQLite")]
        [MemberData(nameof(TransactionScopeOptions))]
        public override void SupportsEnlistingInAmbientTransactions(TransactionScopeOption scopeOption)
        {
            base.SupportsEnlistingInAmbientTransactions(scopeOption);
        }
    }
}
