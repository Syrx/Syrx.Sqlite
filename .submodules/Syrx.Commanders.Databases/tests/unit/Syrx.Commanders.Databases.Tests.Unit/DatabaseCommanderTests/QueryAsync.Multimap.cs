#nullable disable

namespace Syrx.Commanders.Databases.Tests.Unit.DatabaseCommanderTests
{
    public class QueryAsyncMultimap
    {
        [Fact]
        public async Task QueryAsyncDisposesConnectionWhenExecutionFails()
        {
            var method = nameof(QueryAsyncDisposesConnectionWhenExecutionFails);
            var setting = new CommandSetting
            {
                CommandText = "select 1",
                ConnectionAlias = "test"
            };

            var reader = new Mock<IDatabaseCommandReader>();
            reader
                .Setup(x => x.GetCommand(typeof(QueryAsyncMultimap), method))
                .Returns(setting);

            var connection = new Mock<IDbConnection>(MockBehavior.Strict);
            connection.SetupGet(x => x.State).Returns(ConnectionState.Open);
            connection.Setup(x => x.CreateCommand()).Throws(new InvalidOperationException("Unit test"));
            connection.Setup(x => x.Dispose());

            var connector = new Mock<IDatabaseConnector>();
            connector
                .Setup(x => x.CreateConnection(setting))
                .Returns(connection.Object);

            var sut = new DatabaseCommander<QueryAsyncMultimap>(reader.Object, connector.Object);

            await ThrowsAnyAsync<Exception>(() => sut.QueryAsync<int>(method: method));

            connection.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public async Task QueryAsyncMultimapPropagatesCancellationToken()
        {
            var method = nameof(QueryAsyncMultimapPropagatesCancellationToken);
            var setting = new CommandSetting
            {
                CommandText = "select 1 as Id, 2 as Value",
                ConnectionAlias = "test",
                Split = "Value"
            };

            var reader = new Mock<IDatabaseCommandReader>();
            reader
                .Setup(x => x.GetCommand(typeof(QueryAsyncMultimap), method))
                .Returns(setting);

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var connection = new CancellationTrackingDbConnection();

            var connector = new Mock<IDatabaseConnector>();
            connector
                .Setup(x => x.CreateConnection(setting))
                .Returns(connection);

            var sut = new DatabaseCommander<QueryAsyncMultimap>(reader.Object, connector.Object);

            await ThrowsAsync<OperationCanceledException>(() =>
                sut.QueryAsync<FirstType, SecondType, int>((first, second) => first.Id + second.Value, cancellationToken: cts.Token, method: method));

            Equal(cts.Token, connection.ObservedToken);
        }

        private sealed class FirstType
        {
            public int Id { get; set; }
        }

        private sealed class SecondType
        {
            public int Value { get; set; }
        }

        private sealed class CancellationTrackingDbConnection : DbConnection
        {
            private readonly CancellationTrackingDbCommand _command = new();
            private string _connectionString = string.Empty;

            public override string ConnectionString
            {
                get => _connectionString;
                [param: AllowNull]
                set => _connectionString = value ?? string.Empty;
            }
            public override string Database => "test";
            public override string DataSource => "test";
            public override string ServerVersion => "1.0";
            public override ConnectionState State => ConnectionState.Open;
            public CancellationToken ObservedToken => _command.ObservedToken;

            public override void Open()
            {
            }

            public override Task OpenAsync(CancellationToken cancellationToken) => Task.CompletedTask;

            public override void Close()
            {
            }

            public override void ChangeDatabase(string databaseName)
            {
            }

            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
                => throw new NotSupportedException();

            protected override DbCommand CreateDbCommand() => _command;
        }

        private sealed class CancellationTrackingDbCommand : DbCommand
        {
            private readonly CancellationTrackingParameterCollection _parameters = new();

            public CancellationToken ObservedToken { get; private set; }

            public override string CommandText { get; set; } = string.Empty;
            public override int CommandTimeout { get; set; }
            public override CommandType CommandType { get; set; }
            public override bool DesignTimeVisible { get; set; }
            public override UpdateRowSource UpdatedRowSource { get; set; }
            protected override DbConnection DbConnection { get; set; } = null!;
            protected override DbParameterCollection DbParameterCollection => _parameters;
            protected override DbTransaction DbTransaction { get; set; } = null!;

            public override void Cancel()
            {
            }

            public override int ExecuteNonQuery() => throw new NotSupportedException();
            public override object ExecuteScalar() => throw new NotSupportedException();
            public override void Prepare()
            {
            }

            protected override DbParameter CreateDbParameter() => new CancellationTrackingDbParameter();

            protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => new EmptyDbDataReader();

            protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
            {
                ObservedToken = cancellationToken;
                cancellationToken.ThrowIfCancellationRequested();
                return Task.FromResult<DbDataReader>(new EmptyDbDataReader());
            }
        }

        private sealed class CancellationTrackingDbParameter : DbParameter
        {
            public override DbType DbType { get; set; }
            public override ParameterDirection Direction { get; set; }
            public override bool IsNullable { get; set; }
            public override string ParameterName { get; set; }
            public override string SourceColumn { get; set; }
            public override object Value { get; set; }
            public override bool SourceColumnNullMapping { get; set; }
            public override int Size { get; set; }
            public override void ResetDbType()
            {
            }
        }

        private sealed class CancellationTrackingParameterCollection : DbParameterCollection
        {
            private readonly List<DbParameter> _parameters = [];

            public override int Count => _parameters.Count;
            public override object SyncRoot => ((System.Collections.ICollection)_parameters).SyncRoot;

            public override int Add(object value)
            {
                _parameters.Add((DbParameter)value);
                return _parameters.Count - 1;
            }

            public override void AddRange(Array values)
            {
                foreach (var value in values)
                {
                    Add(value!);
                }
            }

            public override void Clear() => _parameters.Clear();
            public override bool Contains(object value) => _parameters.Contains((DbParameter)value);
            public override bool Contains(string value) => _parameters.Any(parameter => parameter.ParameterName == value);
            public override void CopyTo(Array array, int index) => ((System.Collections.ICollection)_parameters).CopyTo(array, index);
            public override System.Collections.IEnumerator GetEnumerator() => _parameters.GetEnumerator();
            public override int IndexOf(object value) => _parameters.IndexOf((DbParameter)value);
            public override int IndexOf(string parameterName) => _parameters.FindIndex(parameter => parameter.ParameterName == parameterName);
            public override void Insert(int index, object value) => _parameters.Insert(index, (DbParameter)value);
            public override void Remove(object value) => _parameters.Remove((DbParameter)value);
            public override void RemoveAt(int index) => _parameters.RemoveAt(index);
            public override void RemoveAt(string parameterName)
            {
                var index = IndexOf(parameterName);
                if (index >= 0)
                {
                    _parameters.RemoveAt(index);
                }
            }

            protected override DbParameter GetParameter(int index) => _parameters[index];
            protected override DbParameter GetParameter(string parameterName) => _parameters[IndexOf(parameterName)];
            protected override void SetParameter(int index, DbParameter value) => _parameters[index] = value;
            protected override void SetParameter(string parameterName, DbParameter value)
            {
                var index = IndexOf(parameterName);
                if (index >= 0)
                {
                    _parameters[index] = value;
                    return;
                }

                _parameters.Add(value);
            }
        }

        private sealed class EmptyDbDataReader : DbDataReader
        {
            public override int FieldCount => 0;
            public override bool HasRows => false;
            public override bool IsClosed => false;
            public override int RecordsAffected => 0;
            public override int Depth => 0;

            public override object this[int ordinal] => throw new IndexOutOfRangeException();
            public override object this[string name] => throw new IndexOutOfRangeException();

            public override bool GetBoolean(int ordinal) => throw new IndexOutOfRangeException();
            public override byte GetByte(int ordinal) => throw new IndexOutOfRangeException();
            public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => throw new IndexOutOfRangeException();
            public override char GetChar(int ordinal) => throw new IndexOutOfRangeException();
            public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => throw new IndexOutOfRangeException();
            public override string GetDataTypeName(int ordinal) => string.Empty;
            public override DateTime GetDateTime(int ordinal) => throw new IndexOutOfRangeException();
            public override decimal GetDecimal(int ordinal) => throw new IndexOutOfRangeException();
            public override double GetDouble(int ordinal) => throw new IndexOutOfRangeException();
            public override Type GetFieldType(int ordinal) => typeof(object);
            public override float GetFloat(int ordinal) => throw new IndexOutOfRangeException();
            public override Guid GetGuid(int ordinal) => throw new IndexOutOfRangeException();
            public override short GetInt16(int ordinal) => throw new IndexOutOfRangeException();
            public override int GetInt32(int ordinal) => throw new IndexOutOfRangeException();
            public override long GetInt64(int ordinal) => throw new IndexOutOfRangeException();
            public override string GetName(int ordinal) => string.Empty;
            public override int GetOrdinal(string name) => -1;
            public override string GetString(int ordinal) => throw new IndexOutOfRangeException();
            public override object GetValue(int ordinal) => throw new IndexOutOfRangeException();
            public override int GetValues(object[] values) => 0;
            public override bool IsDBNull(int ordinal) => true;
            public override bool NextResult() => false;
            public override bool Read() => false;
            public override System.Collections.IEnumerator GetEnumerator() => Array.Empty<object>().GetEnumerator();
        }
    }
}
