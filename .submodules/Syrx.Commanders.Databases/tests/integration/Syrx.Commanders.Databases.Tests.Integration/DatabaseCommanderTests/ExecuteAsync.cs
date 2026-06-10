namespace Syrx.Commanders.Databases.Tests.Integration.DatabaseCommanderTests
{
    public abstract class ExecuteAsync(Fixture fixture)
    {
        private readonly ICommander<Execute> _commander = fixture.ResolveCommander<Execute>();

        [Fact]
        public virtual async Task ExceptionsAreReturnedToCaller()
        {
            var result = await ThrowsAnyAsync<Exception>(() => _commander.ExecuteAsync(new { value = 1 }));
            var expected = fixture.AssertionMessages.Retrieve<ExecuteAsync>(nameof(ExceptionsAreReturnedToCaller));
            result.HasMessage(expected);
        }

        [Fact]
        public virtual async Task SupportParameterlessCalls()
        {
            var result = await _commander.ExecuteAsync<bool>();
            True(result);
        }

        [Fact]
        public virtual async Task SupportsRollbackOnParameterlessCalls()
        {
            // get a count from [dbo].[Poco]
            // try to delete a result.
            //  throw exception
            // check count again. should match.
            var method = $"{nameof(Execute.SupportsRollbackOnParameterlessCalls)}.Count";
            var preCount = _commander.Query<int>(method: method);
            var result = await ThrowsAnyAsync<Exception>(() => _commander.ExecuteAsync<bool>());
            var postCount = _commander.Query<int>(method: method);

            var expected = fixture.AssertionMessages.Retrieve<ExecuteAsync>(nameof(SupportsRollbackOnParameterlessCalls));
            result.HasMessage(expected);
            Equal(preCount, postCount);
        }

        [Fact]
        public virtual async Task SupportsSuppressedDistributedTransactions()
        {
            var one = new ImmutableType(1, Guid.NewGuid().ToString(), 1, DateTime.UtcNow);
            var two = new ImmutableType(2, Guid.NewGuid().ToString(), 2, DateTime.UtcNow);

            var result = await _commander.ExecuteAsync(() =>
            {
                var a = _commander.Execute(one) ? one : null;//, (a) => a);
                var b = _commander.Execute(two) ? two : null; // (b) => b);

                return new ImmutableTwoType<ImmutableType, ImmutableType, ImmutableType>(a, b);
            });

            NotNull(result);
            NotNull(result.One);
            NotNull(result.Two);

            Same(one, result.One);
            Same(two, result.Two);
        }

        [Fact]
        public virtual async Task SupportsTransactionRollback()
        {
            var method = $"{nameof(Execute.SupportsTransactionRollback)}.Count";

            var model = new ImmutableType(1, Guid.NewGuid().ToString(), int.MaxValue, DateTime.UtcNow);

            var result = await ThrowsAnyAsync<Exception>(() => _commander.ExecuteAsync(model));
            var expected = fixture.AssertionMessages.Retrieve<ExecuteAsync>(nameof(SupportsTransactionRollback));
            result.HasMessage(expected);

            // check if the result has been rolled back.
            // ReSharper disable once ExplicitCallerInfoArgument
            var record = _commander.Query<ImmutableType>(new { model.Name }, method: method);
            NotNull(record);
            False(record.Any());
        }

        [Theory]
        [MemberData(nameof(TransactionScopeOptions))]
        public virtual async Task SupportsEnlistingInAmbientTransactions(TransactionScopeOption scopeOption)
        {
            var name = Enum.GetName(typeof(TransactionScopeOption), scopeOption);

            var one = new ImmutableType(1, $"{name}--{Guid.NewGuid()}", 1, DateTime.UtcNow);
            var two = new ImmutableType(2, $"{name}--{Guid.NewGuid()}", 2, DateTime.UtcNow);

            var result = await _commander.ExecuteAsync(() =>
            {
                var a = _commander.Execute(one) ? one : null;//, (a) => a);
                var b = _commander.Execute(two) ? two : null; // (b) => b);

                return new ImmutableTwoType<ImmutableType, ImmutableType, ImmutableType>(a, b);
            }, scopeOption);

            NotNull(result);
            NotNull(result.One);
            NotNull(result.Two);

            Same(one, result.One);
            Same(two, result.Two);
        }


        [Fact]
        public virtual async Task SuccessfullyWithResponse()
        {
            var random = new Random();
            var overload = $"{nameof(SuccessfullyWithResponse)}.Response";
            var one = new ImmutableType(500, $"{nameof(ImmutableType)}.{random.Next(int.MaxValue)}", random.Next(int.MaxValue), DateTime.UtcNow);
            var result = await _commander.ExecuteAsync(() =>
            {
                return _commander.Execute(one) ?
                        _commander.Query<ImmutableType>(new { name = one.Name }, overload).SingleOrDefault()
                        : null;
            }
            );

            NotEqual(one, result);
            NotEqual(one.Id, result.Id);
            Equal(one.Name, result.Name);
            Equal(one.Value, result.Value);
        }

        [Fact]
        public virtual async Task Successful()
        {
            var random = new Random();
            var one = new ImmutableType(500, nameof(ImmutableType), random.Next(int.MaxValue), DateTime.UtcNow);
            var result = await _commander.ExecuteAsync(one);
            True(result);
        }

        [Theory]
        [MemberData(nameof(ModelGenerators.Multimap.SingleTypeData), MemberType = typeof(ModelGenerators.Multimap))]
        public virtual async Task SingleType<T1>(SingleType<T1> input)
        {
            var result = await _commander.ExecuteAsync(input.One);
            True(result);
        }

        [Fact]
        public virtual async Task NullModelThrowsArgumentNullException()
        {
            var model = new { Name = Guid.NewGuid() };
            model = null;
            var result = await ThrowsAsync<ArgumentNullException>(() => _commander.ExecuteAsync(model));
            result.ArgumentNull(nameof(model));
        }

        public static TheoryData<TransactionScopeOption> TransactionScopeOptions => new()
            {
            TransactionScopeOption.Required,
            TransactionScopeOption.RequiresNew,
            TransactionScopeOption.Suppress
            };

    }

}
