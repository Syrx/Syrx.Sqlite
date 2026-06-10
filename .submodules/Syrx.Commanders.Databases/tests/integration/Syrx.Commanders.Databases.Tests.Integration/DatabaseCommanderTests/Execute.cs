namespace Syrx.Commanders.Databases.Tests.Integration.DatabaseCommanderTests
{
    // although I *could* use [CallerMemberName] for resolving the method names
    // for AssertionMessage lookup, I'd rather not have to convert Fact tests to 
    // Theory tests to accommodate and will continue to use nameof instead. 

    public abstract class Execute 
    {
        private readonly ICommander<Execute> _commander;
        private readonly Fixture _fixture;

        public Execute(Fixture fixture)
        {
            _commander = fixture.ResolveCommander<Execute>();
            _fixture = fixture;
        }

        [Fact]
        public virtual void ExceptionsAreReturnedToCaller()
        {
            var result = ThrowsAny<Exception>(() => _commander.Execute(new { value = 1 }));
            var expected = _fixture.AssertionMessages.Retrieve<Execute>(nameof(ExceptionsAreReturnedToCaller));
            result.HasMessage(expected);
        }
                
        [Fact]
        public virtual void SupportParameterlessCalls()
        {
            var result = _commander.Execute<bool>();
            True(result);
        }

        [Fact]
        public virtual void SupportsRollbackOnParameterlessCalls()
        {
            // get a count from [dbo].[Poco]
            // try to delete a result.
            //  throw exception
            // check count again. should match.
            var method = $"{nameof(SupportsRollbackOnParameterlessCalls)}.Count";
            var preCount = _commander.Query<int>(method: method);
            var result = ThrowsAny<Exception>(() => _commander.Execute<bool>());
            var postCount = _commander.Query<int>(method: method);

            var expected = _fixture.AssertionMessages.Retrieve<Execute>(nameof(SupportsRollbackOnParameterlessCalls));
            result.HasMessage(expected);

            Equal(preCount, postCount);
        }

        [Fact]
        public virtual void SupportsSuppressedDistributedTransactions()
        {
            var one = new ImmutableType(1, Guid.NewGuid().ToString(), 1, DateTime.UtcNow);
            var two = new ImmutableType(2, Guid.NewGuid().ToString(), 2, DateTime.UtcNow);

            var result = _commander.Execute(() =>
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
        public virtual void SupportsTransactionRollback()
        {
            var method = $"{nameof(SupportsTransactionRollback)}.Count";

            var model = new ImmutableType(1, Guid.NewGuid().ToString(), int.MaxValue, DateTime.UtcNow);

            _fixture.AssertionMessages.Retrieve<Execute>(nameof(SupportsTransactionRollback));

            var result = ThrowsAny<Exception>(() => _commander.Execute(model));
            //var expected = $"Arithmetic overflow error converting expression to data type float.{Environment.NewLine}The statement has been terminated.";
            var expected = _fixture.AssertionMessages.Retrieve<Execute>(nameof(SupportsTransactionRollback));
            result.HasMessage(expected);

            // check if the result has been rolled back.
            // ReSharper disable once ExplicitCallerInfoArgument
            var record = _commander.Query<ImmutableType>(new { model.Name }, method: method);
            NotNull(record);
            False(record.Any());
        }

        [Theory]
        [MemberData(nameof(TransactionScopeOptions))]
        public virtual void SupportsEnlistingInAmbientTransactions(TransactionScopeOption scopeOption)
        {
            var name = Enum.GetName(typeof(TransactionScopeOption), scopeOption);

            var one = new ImmutableType(1, $"{name}--{Guid.NewGuid()}", 1, DateTime.UtcNow);
            var two = new ImmutableType(2, $"{name}--{Guid.NewGuid()}", 2, DateTime.UtcNow);

            var result = _commander.Execute(() =>
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
        public virtual void SuccessfullyWithResponse()
        {
            var random = new Random();
            var overload = $"{nameof(SuccessfullyWithResponse)}.Response";
            var one = new ImmutableType(500, $"{ Guid.NewGuid() }", random.Next(int.MaxValue), DateTime.UtcNow);
            var result = _commander.Execute(() =>
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
        public virtual void Successful()
        {
            var random = new Random();
            var one = new ImmutableType(500, nameof(ImmutableType), random.Next(int.MaxValue), DateTime.UtcNow);
            var result = _commander.Execute(one);
            True(result);
        }

        [Theory]
        [MemberData(nameof(ModelGenerators.Multimap.SingleTypeData), MemberType = typeof(ModelGenerators.Multimap))]
        public virtual void SingleType<T1>(SingleType<T1> input)
        {
            var result = _commander.Execute(input.One);
            True(result);
        }

        [Fact]
        public virtual void NullModelThrowsArgumentNullException()
        {
            var model = new { Name = Guid.NewGuid() };
            model = null;
            var result = Throws<ArgumentNullException>(() => _commander.Execute(model));
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
