namespace Syrx.Sqlite.Tests.Integration.Fixtures
{
    public sealed class SqliteFixture : Fixture, IAsyncLifetime
    {
        private readonly string _databasePath;
        private IServiceProvider? _serviceProvider;
        private bool _disposed;

        public SqliteFixture()
        {
            _databasePath = Path.Combine(Path.GetTempPath(), $"syrx_sqlite_{Guid.NewGuid():N}.db");
        }

        public Repositories.SqliteSmokeRepository Repository =>
            _serviceProvider?.GetRequiredService<Repositories.SqliteSmokeRepository>()
            ?? throw new InvalidOperationException("Fixture is not initialized.");

        public Task InitializeAsync()
        {
            var connectionString = $"Data Source={_databasePath};Pooling=False;";
            var alias = SqliteCommandStrings.Alias;

            // Register Dapper type handlers for SQLite type compatibility.
            // SQLite returns INTEGER as Int64, REAL as Double, TEXT for datetime strings.
            // ImmutableType expects int, decimal, DateTime — so we register handlers.
            SqlMapper.AddTypeHandler(new SqliteIntTypeHandler());
            SqlMapper.AddTypeHandler(new SqliteDecimalTypeHandler());
            SqlMapper.AddTypeHandler(new SqliteDateTimeTypeHandler());

            var services = new ServiceCollection();
            services.UseSyrx(b => b.SetupSqlite(alias, connectionString));
            services.AddSingleton<Repositories.SqliteSmokeRepository>();
            _serviceProvider = services.BuildServiceProvider();

            Install(() => _serviceProvider);
            Installer.SetupDatabase(ResolveCommander<DatabaseBuilder>());

            // Set up smoke table via the Repository
            Repository.CreateTable();
            Repository.DeleteAll();
            Repository.Insert(1, "alpha");

            // Set assertion messages for SQLite-specific error strings.
            // Microsoft.Data.Sqlite formats messages as: "SQLite Error {code}: '{msg}'."
            AssertionMessages.Add<Execute>(nameof(Execute.ExceptionsAreReturnedToCaller), "SQLite Error 1: 'no such table: does_not_exist'.");
            AssertionMessages.Add<Execute>(nameof(Execute.SupportsRollbackOnParameterlessCalls), "SQLite Error 1: 'no such table: does_not_exist'.");
            AssertionMessages.Add<Execute>(nameof(Execute.SupportsTransactionRollback), "SQLite Error 19: 'CHECK constraint failed: check_poco_value'.");

            AssertionMessages.Add<ExecuteAsync>(nameof(ExecuteAsync.ExceptionsAreReturnedToCaller), "SQLite Error 1: 'no such table: does_not_exist'.");
            AssertionMessages.Add<ExecuteAsync>(nameof(ExecuteAsync.SupportsRollbackOnParameterlessCalls), "SQLite Error 1: 'no such table: does_not_exist'.");
            AssertionMessages.Add<ExecuteAsync>(nameof(ExecuteAsync.SupportsTransactionRollback), "SQLite Error 19: 'CHECK constraint failed: check_poco_value'.");

            AssertionMessages.Add<Query>(nameof(Query.ExceptionsAreReturnedToCaller), "SQLite Error 1: 'no such table: does_not_exist'.");
            AssertionMessages.Add<QueryAsync>(nameof(QueryAsync.ExceptionsAreReturnedToCaller), "SQLite Error 1: 'no such table: does_not_exist'.");

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            (_serviceProvider as IDisposable)?.Dispose();
            _serviceProvider = null;

            if (!File.Exists(_databasePath))
            {
                return;
            }

            for (var attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    File.Delete(_databasePath);
                    return;
                }
                catch (IOException) when (attempt < 4)
                {
                    Thread.Sleep(50);
                }
            }
        }

        private sealed class SqliteIntTypeHandler : SqlMapper.TypeHandler<int>
        {
            public override int Parse(object value) => Convert.ToInt32(value);
            public override void SetValue(IDbDataParameter parameter, int value) => parameter.Value = value;
        }

        private sealed class SqliteDecimalTypeHandler : SqlMapper.TypeHandler<decimal>
        {
            public override decimal Parse(object value) => Convert.ToDecimal(value);
            public override void SetValue(IDbDataParameter parameter, decimal value) => parameter.Value = (double)value;
        }

        private sealed class SqliteDateTimeTypeHandler : SqlMapper.TypeHandler<DateTime>
        {
            public override DateTime Parse(object value)
            {
                if (value is string s)
                    return DateTime.Parse(s, System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None);
                return Convert.ToDateTime(value);
            }

            public override void SetValue(IDbDataParameter parameter, DateTime value)
                => parameter.Value = value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
