
namespace Syrx.Commanders.Databases.Tests.Unit.DatabaseCommanderTests
{
    public class ExecuteAsync
    {
        [Fact]
        public async Task ExecuteAsyncCoreUsesDbConnectionOpenAsync()
        {
            var method = nameof(ExecuteAsyncCoreUsesDbConnectionOpenAsync);
            var setting = new CommandSetting
            {
                CommandText = "update dbo.Test set Value = 1",
                ConnectionAlias = "test"
            };

            var reader = new Mock<IDatabaseCommandReader>();
            reader.Setup(x => x.GetCommand(typeof(ExecuteAsync), method)).Returns(setting);

            var connection = new TrackingDbConnection();
            var connector = new Mock<IDatabaseConnector>();
            connector.Setup(x => x.CreateConnection(setting)).Returns(connection);

            var sut = new DatabaseCommander<ExecuteAsync>(reader.Object, connector.Object);

            await ThrowsAsync<InvalidOperationException>(() => sut.ExecuteAsync<int>(cancellationToken: default, method: method));

            True(connection.OpenAsyncCalled);
            False(connection.OpenCalled);
        }

        [Fact]
        public async Task ExecuteAsyncMapOverloadPassesCancellationToken()
        {
            var reader = new Mock<IDatabaseCommandReader>();
            var connector = new Mock<IDatabaseConnector>();
            var sut = new DatabaseCommander<ExecuteAsync>(reader.Object, connector.Object);

            using var cts = new CancellationTokenSource();
            CancellationToken observed = default;

            var result = await sut.ExecuteAsync<int>(
                async token =>
                {
                    observed = token;
                    await Task.Yield();
                    return 42;
                },
                cancellationToken: cts.Token);

            Equal(42, result);
            Equal(cts.Token, observed);
        }

        private sealed class TrackingDbConnection : DbConnection
        {
            private ConnectionState _state = ConnectionState.Closed;

            public bool OpenCalled { get; private set; }
            public bool OpenAsyncCalled { get; private set; }

            [AllowNull]
            public override string ConnectionString { get; set; } = string.Empty;
            public override string Database => "test";
            public override string DataSource => "test";
            public override string ServerVersion => "1.0";
            public override ConnectionState State => _state;

            public override void Open()
            {
                OpenCalled = true;
                _state = ConnectionState.Open;
            }

            public override Task OpenAsync(CancellationToken cancellationToken)
            {
                OpenAsyncCalled = true;
                cancellationToken.ThrowIfCancellationRequested();
                _state = ConnectionState.Open;
                return Task.CompletedTask;
            }

            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
                => throw new InvalidOperationException("Unit test");

            public override void Close() => _state = ConnectionState.Closed;

            public override void ChangeDatabase(string databaseName)
            {
            }

            protected override DbCommand CreateDbCommand()
                => throw new NotImplementedException();
        }
    }
}
