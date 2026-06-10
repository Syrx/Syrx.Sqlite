namespace Syrx.Sqlite.Tests.Integration
{
    [Collection(nameof(SqliteFixtureCollection))]
    public class SqliteSmokeRepositoryTests
    {
        private readonly Fixtures.SqliteFixture _fixture;

        public SqliteSmokeRepositoryTests(Fixtures.SqliteFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void GetNameById_ReturnsSeededRow()
        {
            ResetState();

            var name = _fixture.Repository.GetNameById(1);

            Assert.Equal("alpha", name);
        }

        [Fact]
        public void Insert_ThenGetNameById_ReturnsInsertedValue()
        {
            ResetState();
            _fixture.Repository.Insert(2, "beta");

            var name = _fixture.Repository.GetNameById(2);

            Assert.Equal("beta", name);
        }

        [Fact]
        public void DeleteAll_RemovesRows()
        {
            ResetState();

            _fixture.Repository.DeleteAll();
            var name = _fixture.Repository.GetNameById(1);

            Assert.Null(name);
        }

        [Fact]
        public void GetNameById_ForMissingRow_ReturnsNull()
        {
            ResetState();

            var name = _fixture.Repository.GetNameById(999);

            Assert.Null(name);
        }

        [Fact]
        public void Insert_WithInvalidId_Throws()
        {
            var action = () => _fixture.Repository.Insert(0, "bad");

            var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
            Assert.Contains("id must be greater than zero", exception.Message);
        }

        [Fact]
        public void Insert_WithEmptyName_Throws()
        {
            var action = () => _fixture.Repository.Insert(1, string.Empty);

            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Contains("name is required", exception.Message);
        }

        [Fact]
        public void GetNameById_WithInvalidId_Throws()
        {
            var action = () => _fixture.Repository.GetNameById(0);

            var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
            Assert.Contains("id must be greater than zero", exception.Message);
        }

        [Fact]
        public void CreateTable_CanRunMultipleTimes()
        {
            _fixture.Repository.CreateTable();
            _fixture.Repository.CreateTable();
            _fixture.Repository.DeleteAll();
            _fixture.Repository.Insert(1, "alpha");

            var name = _fixture.Repository.GetNameById(1);

            Assert.Equal("alpha", name);
        }

        [Fact]
        public void DeleteAll_IsIdempotent()
        {
            ResetState();

            _fixture.Repository.DeleteAll();
            _fixture.Repository.DeleteAll();

            Assert.Null(_fixture.Repository.GetNameById(1));
        }

        [Fact]
        public void Insert_AllowsSpecialCharacters()
        {
            ResetState();
            const string expected = "name with symbols !@#$%^&*()_+-=[]{}|;:'\",.<>/?";

            _fixture.Repository.Insert(2, expected);

            Assert.Equal(expected, _fixture.Repository.GetNameById(2));
        }

        [Fact]
        public void Insert_AllowsLongNames()
        {
            ResetState();
            var expected = new string('n', 1024);

            _fixture.Repository.Insert(2, expected);

            Assert.Equal(expected, _fixture.Repository.GetNameById(2));
        }

        [Fact]
        public void Insert_MultipleRows_CanBeRetrievedIndependently()
        {
            ResetState();
            _fixture.Repository.Insert(2, "beta");
            _fixture.Repository.Insert(3, "gamma");

            Assert.Equal("alpha", _fixture.Repository.GetNameById(1));
            Assert.Equal("beta", _fixture.Repository.GetNameById(2));
            Assert.Equal("gamma", _fixture.Repository.GetNameById(3));
        }

        [Fact]
        public void Insert_WithWhitespaceName_Throws()
        {
            var action = () => _fixture.Repository.Insert(1, "   ");

            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Contains("name is required", exception.Message);
        }

        [Fact]
        public void Insert_WithNullName_Throws()
        {
            var action = () => _fixture.Repository.Insert(1, null!);

            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Contains("name is required", exception.Message);
        }

        [Fact]
        public void Insert_WithDuplicateId_ThrowsSqliteException()
        {
            ResetState();

            var action = () => _fixture.Repository.Insert(1, "duplicate");

            Assert.Throws<Microsoft.Data.Sqlite.SqliteException>(action);
        }

        [Fact]
        public void Reinsert_AfterDeleteAll_WithSameId_Succeeds()
        {
            ResetState();
            _fixture.Repository.DeleteAll();

            _fixture.Repository.Insert(1, "reinserted");

            Assert.Equal("reinserted", _fixture.Repository.GetNameById(1));
        }

        [Fact]
        public void GetNameById_WithLargeId_ReturnsNull()
        {
            ResetState();

            var name = _fixture.Repository.GetNameById(int.MaxValue);

            Assert.Null(name);
        }

        [Fact]
        public void CreateTable_AfterExistingRows_PreservesData()
        {
            ResetState();
            _fixture.Repository.Insert(2, "beta");

            _fixture.Repository.CreateTable();

            Assert.Equal("alpha", _fixture.Repository.GetNameById(1));
            Assert.Equal("beta", _fixture.Repository.GetNameById(2));
        }

        [Fact]
        public void Insert_ManyRows_RetrievesLastRow()
        {
            ResetState();
            for (var id = 2; id <= 50; id++)
            {
                _fixture.Repository.Insert(id, $"name-{id}");
            }

            var name = _fixture.Repository.GetNameById(50);

            Assert.Equal("name-50", name);
        }

        private void ResetState()
        {
            _fixture.Repository.CreateTable();
            _fixture.Repository.DeleteAll();
            _fixture.Repository.Insert(1, "alpha");
        }
    }
}
