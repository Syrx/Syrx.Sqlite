# Transaction Handling

How Syrx.Commanders.Databases handles transactions for data modification commands.

---

## Transaction Policy

### Query Operations (Read-Only)

**Queries do NOT use transactions:**

```csharp
// These methods do NOT wrap queries in transactions:
await _commander.QueryAsync<User>();
await _commander.QueryAsync<User, Profile, UserProfile>(...);
await _commander.QueryAsync<User, IEnumerable<Profile>, ...>(...);

// Each query:
├─ Gets connection from pool
├─ Executes without explicit transaction
├─ Returns results
└─ Connection returned to pool
```

**Rationale**: Queries don't modify data. Transactions add overhead.

### Execute Operations (Write)

**Execute operations ARE wrapped in transactions:**

```csharp
// These methods wrap in transactions:
await _commander.ExecuteAsync(user);  // INSERT/UPDATE/DELETE

// Execution flow:
├─ Retrieve connection from pool
├─ BEGIN TRANSACTION
├─ Execute command
├─ If success: Commit
├─ If exception: Rollback (automatic)
└─ Return success/failure
```

**Rationale**: Protect data integrity for modifications.

---

## Transaction Isolation Level

### Configuration

Isolation level is specified in configuration:

```json
{
    "commands": {
    "CreateUser": {
            "commandText": "INSERT INTO Users ...",
            "connectionAlias": "Default",
            "isolationLevel": "Serializable"  // ← Default
        }
    }
}
```

### Supported Levels

| Level | Isolation | Dirty Reads | Non-Repeatable Reads | Phantoms |
|-------|-----------|-------------|----------------------|----------|
| `ReadUncommitted` | Lowest | Yes | Yes | Yes |
| `ReadCommitted` | Default (SQL Server) | No | Yes | Yes |
| `RepeatableRead` | Medium | No | No | Yes |
| `Serializable` | **Highest (Syrx default)** | No | No | No |

### Default Behavior

**Syrx defaults to `Serializable`** — the strictest isolation level.

```csharp
// Configuration (in CommandSetting):
public IsolationLevel IsolationLevel { get; init; } = IsolationLevel.Serializable;
```

**Rationale**: Safest default for business applications. Can lower if needed for performance.

---

## Transaction Lifecycle

### Successful Execution

```
Execute request
  ↓
Retrieve connection
  ↓
Set isolation level
  ↓
BEGIN TRANSACTION
  ↓
Execute command (INSERT/UPDATE/DELETE)
  ↓
Check: Success (no exception)?
  ├─ YES:
  │   │ Affected rows > 0?
  │   ├─ YES: COMMIT (return true)
  │   └─ NO: COMMIT (return false)
  └─ NO: ROLLBACK (throw exception)
  ↓
Return to caller
```

### Exception Handling

```
Execute request
  ↓
[... setup and execution ...]
  ↓
Exception occurs during execution
  ↓
ROLLBACK (automatic)
  ↓
Connection returned to pool
  ↓
Exception re-thrown to repository
  ↓
Repository handles exception
```

**Policy**: Exceptions bubble up; no silent catches. Repository must handle.

---

## Return Values

Execute operations return `bool`:

```csharp
// Sync version
bool Execute<TResult>(TResult model, [CallerMemberName] string method = null);

// Async version
Task<bool> ExecuteAsync<TResult>(
    TResult model, 
    CancellationToken cancellationToken = default,
    [CallerMemberName] string method = null);

// Semantics:
// true = Command executed successfully (affected rows > 0)
// false = Command executed but no rows affected
// Exception = Database error, constraints violation, etc.
```

### Interpreting Results

```csharp
// Repository usage:
var result = await _commander.ExecuteAsync(user);

if (result)
{
    // Command executed and affected rows
    return user;
}
else
{
    // Command executed but affected 0 rows
    // (e.g., UPDATE where Id doesn't exist)
    throw new NotFoundException();
}
```

---

## Multi-Statement Transactions

For multiple commands that must succeed or fail together:

**Syrx does NOT support multi-statement transaction patterns directly.**

```csharp
// This won't work as a single transaction:
await _commander.ExecuteAsync(user);        // Command 1 - separate transaction
await _commander.ExecuteAsync(profile);    // Command 2 - separate transaction
// ✗ No guarantee both succeed or both fail
```

### Workaround: Use Stored Procedures

Wrap multiple statements in a stored procedure:

```sql
-- SQL Stored Procedure
CREATE PROCEDURE sp_CreateUserWithProfile
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @Bio NVARCHAR(MAX)
AS
BEGIN
    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Users (Name, Email) VALUES (@Name, @Email)
        INSERT INTO Profiles (UserId, Bio) 
            VALUES (SCOPE_IDENTITY(), @Bio)
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW
    END CATCH
END
```

```csharp
// Repository method
public async Task<bool> CreateUserWithProfileAsync(User user)
{
    return await _commander.ExecuteAsync(user);
}

// Configuration
{
  "CreateUserWithProfileAsync": {
    "commandText": "sp_CreateUserWithProfile",
        "commandType": "StoredProcedure",
        "connectionAlias": "Default",
        "isolationLevel": "Serializable"
    }
}
```

---

## Error Handling Patterns

### Constraint Violations

```csharp
try
{
    var result = await _commander.ExecuteAsync(user);
    return result;
}
catch (SqlException ex) when (ex.Number == 2627)  // Unique constraint
{
    throw new DuplicateUserException(user.Email);
}
catch (SqlException ex) when (ex.Number == 547)   // Foreign key
{
    throw new InvalidDepartmentException();
}
catch (SqlException ex)
{
    throw new DataAccessException("Unexpected database error", ex);
}
```

### Timeout

Configuration includes `CommandTimeout` (seconds):

```json
{
  "CreateUser": {
        "commandText": "INSERT INTO Users ...",
        "connectionAlias": "Default",
        "commandTimeout": 30  // ← Timeout in seconds
    }
}
```

If execution exceeds timeout, Dapper throws `TimeoutException`.

---

## Distributed Transactions (Advanced)

Syrx does not manage distributed transactions (DTC/MSDTC).

For distributed scenarios:

```csharp
// Manual distributed transaction management (if needed):
using (var scope = new TransactionScope(
    TransactionScopeOption.Required,
    new TransactionOptions 
    { 
        IsolationLevel = System.Transactions.IsolationLevel.Serializable 
    }))
{
    var result = await _commander.ExecuteAsync(user);  // Participates in ambient transaction
    var result2 = await _commander_db2.ExecuteAsync(entity);  // Another database
    
    if (result && result2)
    {
        scope.Complete();  // Commits both
    }
    // else: automatic rollback on dispose
}
```

---

## Best Practices

1. **Keep transactions short**: Don't hold open; return quickly
2. **Use appropriate isolation level**: `Serializable` is default; lower if performance issues
3. **Use stored procedures for complex multi-statement operations**
4. **Handle exceptions explicitly**: Exceptions bubble up; repository must decide action
5. **Monitor command timeout**: Increase if needed for long operations
6. **Test rollback scenarios**: Verify error handling works correctly

---

## Related Pages

- [Architecture Overview](index.md)
- [Execute Operations](../projects/commanders-databases.md#execute-operations)



