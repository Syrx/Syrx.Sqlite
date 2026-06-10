namespace Syrx.Sqlite.Tests.Integration.Repositories
{
    public class SqliteSmokeRepository(ICommander<SqliteSmokeRepository> commander)
    {
        private readonly ICommander<SqliteSmokeRepository> _commander = commander;

        public void CreateTable() => _commander.Execute<bool>();

        public void DeleteAll() => _commander.Execute<bool>();

        public void Insert(int id, string name)
        {
            Throw<ArgumentOutOfRangeException>(id > 0, "id must be greater than zero");
            Throw<ArgumentException>(!string.IsNullOrWhiteSpace(name), "name is required");
            _commander.Execute(new { Id = id, Name = name });
        }

        public string? GetNameById(int id)
        {
            Throw<ArgumentOutOfRangeException>(id > 0, "id must be greater than zero");
            return _commander.Query<string>(new { Id = id }).SingleOrDefault();
        }
    }
}
