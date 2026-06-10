# Syrx Data Access Implementation Examples

These examples are framework-oriented and reusable across repositories.

## Example 1: Configuration-Driven Mapping (Builder)

```csharp
var settings = CommanderSettingsBuilderExtensions.Build(builder => builder
    .AddConnectionString("Default", connectionString)
    .AddCommand(ns => ns.ForType<UserRepository>(type => type
        .ForMethod(nameof(UserRepository.RetrieveAsync), cmd => cmd
            .UseConnectionAlias("Default")
            .UseCommandText("SELECT Id, Email, IsActive FROM dbo.Users WHERE Id = @id"))
        .ForMethod(nameof(UserRepository.RetrieveAllAsync), cmd => cmd
            .UseConnectionAlias("Default")
            .UseCommandText("SELECT Id, Email, IsActive FROM dbo.Users ORDER BY Id OFFSET @offset ROWS FETCH NEXT @size ROWS ONLY")))));
```

Use when you want strong central configuration ownership and no command literals in repositories.

## Example 2: CommandStrings + Installer Style

```csharp
public static partial class CommandStrings
{
    public static class User
    {
        public const string RetrieveById = "SELECT Id, Email, IsActive FROM dbo.Users WHERE Id = @Id";
        public const string RetrievePaged = "SELECT Id, Email, IsActive FROM dbo.Users ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        public const string Save = "UPDATE dbo.Users SET Email = @Email, IsActive = @IsActive WHERE Id = @Id";
    }
}
```

```csharp
public static class RepositoryInstaller
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
```

Use when your architecture standardizes command constants and explicit installer ownership.

## Example 3: Multi-Mapping

```csharp
var result = await _commander.QueryAsync<OrderRow, LineRow, OrderDto>(
    (order, line) =>
    {
        order.Lines ??= new List<LineRow>();
        if (line != null)
            order.Lines.Add(line);
        return new OrderDto(order.Id, order.CustomerId, order.Lines);
    },
    new { id },
    cancellationToken);
```

Use when one joined row contains data from two or more models.

## Example 4: Multiple Result Sets

```csharp
var models = await _commander.QueryAsync<OrderRow, LineRow, OrderDto>(
    (orders, lines) =>
    {
        var order = orders.FirstOrDefault();
        if (order == null) return Enumerable.Empty<OrderDto>();
        return new[] { new OrderDto(order.Id, order.CustomerId, lines.ToList()) };
    },
    new { id },
    cancellationToken);
```

Use when batching independent selects into one round trip.

## Example 5: Unit Test Pattern

```csharp
[Fact]
public async Task WhenCommanderReturnsNoRowsRetrieveAsyncReturnsNull()
{
    var commander = new Mock<ICommander<UserRepository>>();

    commander.Setup(x => x.QueryAsync<User>(
            It.IsAny<object>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()))
        .ReturnsAsync(Enumerable.Empty<User>());

    var repository = new UserRepository(commander.Object);
    var result = await repository.RetrieveAsync(Guid.NewGuid(), CancellationToken.None);

    Assert.Null(result);
}
```
