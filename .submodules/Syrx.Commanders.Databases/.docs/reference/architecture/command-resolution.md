# Command Resolution

How Syrx.Commanders.Databases automatically maps repository method calls to configured SQL commands.

---

## Resolution Process

### The Three-Part Key

Every database command is uniquely identified by three components:

```
Namespace     : MyApp.Data
Type          : UserRepository
Method        : RetrieveAsync
                ↓↓↓
Configuration Key: "MyApp.Data.UserRepository.RetrieveAsync"
```

### Resolution Steps

```
1. Repository Method Invoked
   └─ UserRepository.RetrieveAsync(id: 42)

2. ICommander<T> Receives Call
   └─ _commander.QueryAsync<User>(new { id })

3. [CallerMemberName] Captures Method Name
    └─ Compiler magic: method parameter = "RetrieveAsync" (zero reflection)

4. Build Configuration Key
   └─ Key = $"{typeof(TRepository).FullName}.{method}"
    └─ Key = "MyApp.Data.UserRepository.RetrieveAsync"

5. Check Command Cache
   └─ _commandCache.TryGetValue(key, out var cached)
   └─ If found: use cached CommandSetting (goto Step 7)
   └─ If miss: goto Step 6

6. Resolve from Settings (First Time Only)
   └─ IDatabaseCommandReader.GetCommand(type, method)
   └─ Search: namespaces[].types[].commands[]
   └─ Result: CommandSetting
   └─ Cache: _commandCache.GetOrAdd(key, setting)

7. Build Dapper CommandDefinition
   └─ new CommandDefinition(
        commandText: setting.CommandText,
        parameters: parameters,
        transaction: transaction,    // if Execute
        commandTimeout: setting.CommandTimeout,
        commandType: setting.CommandType,
        flags: setting.Flags,
        cancellationToken: cancellationToken)

8. Execute via Dapper
   └─ connection.QueryAsync<T>(commandDefinition)
   └─ Result: IEnumerable<T>

9. Return to Repository
   └─ IEnumerable<T> returned (repository method handles filtering, etc.)
```

---

## Configuration Matching

### Namespace Resolution

The namespace in configuration must match your C# namespace exactly:

```csharp
// C# Code:
namespace MyApp.Data.Repositories {
    public class UserRepository { }
}

// Configuration must have:
{
    "namespaces": [{
        "namespace": "MyApp.Data.Repositories",  // ← Must match exactly
        "types": [...]
    }]
}
```

### Type Name Matching

The type name in configuration must match your class name exactly (case-sensitive):

```csharp
// C# Code:
public class UserRepository { }

// Configuration:
{
    "types": [{
        "name": "UserRepository",  // ← Must match exactly
        "commands": { ... }
    }]
}

// These won't work:
"name": "userRepository"      // ✗ Wrong case
"name": "User_Repository"     // ✗ Different name
"name": "UserRepositoryImpl"   // ✗ Different name
```

### Method Name Matching

The method name key in configuration must match your C# method name exactly:

```csharp
// C# Code:
public async Task<User?> RetrieveAsync(int id, CancellationToken cancellationToken = default) { }

// Configuration:
{
    "commands": {
        "RetrieveAsync": {              // ← Must match exactly
            "commandText": "SELECT ...",
            "connectionAlias": "Default"
        }
    }
}

// These won't match:
"RetrieveAllAsync"                  // ✗ Different method name
"retrieveasync"                    // ✗ Wrong case
"RetrieveAsync "                    // ✗ Has trailing space
```

If you intentionally overload repository methods, prefer distinct names such as `RetrieveAllAsync`. When overloads are unavoidable, pass an explicit `method:` value on the direct `_commander` call and match that value exactly in configuration.

---

## [CallerMemberName] - Zero Reflection Magic

Behind the scenes, Syrx uses `[CallerMemberName]` to capture method names:

```csharp
public async Task<IEnumerable<TResult>> QueryAsync<TResult>(
    object? parameters = null,
    CancellationToken cancellationToken = default,
    [CallerMemberName] string method = null)  // ← Compiler fills this automatically
{
    // At this point, 'method' contains the calling method's name
    // No reflection needed - it's compile-time magic
    
    var cacheKey = $"{_typeFullName}.{method}";
    var setting = _commandCache.GetOrAdd(cacheKey, 
        _ => _reader.GetCommand(_type, method));
    
    // ... execute with setting
}
```

### Requirement: Direct Call

`[CallerMemberName]` only works when you call `_commander` directly from your repository method:

```csharp
// ✅ WORKS
public async Task<User?> RetrieveAsync(int id, CancellationToken cancellationToken = default)
{
    return (await _commander.QueryAsync<User>(new { id }, cancellationToken)).FirstOrDefault();
    //     method name = "RetrieveAsync" ✅
}

// ✗ DOESN'T WORK
private async Task<IEnumerable<User>> InternalRetrieve(CancellationToken cancellationToken = default)
{
    return await _commander.QueryAsync<User>(cancellationToken: cancellationToken);  // method name = "InternalRetrieve" ✗
}

public async Task<User?> RetrieveAsync(int id, CancellationToken cancellationToken = default)
{
    return (await InternalRetrieve(cancellationToken)).FirstOrDefault();  // Won't resolve to RetrieveAsync
}

// ✅ WORKAROUND: Call directly
public async Task<User?> RetrieveAsync(int id, CancellationToken cancellationToken = default)
{
    return (await _commander.QueryAsync<User>(new { id }, cancellationToken)).FirstOrDefault();
    //     Direct call ✅
}

public async Task<IEnumerable<User>> RetrieveAllAsync(
    int page = 1,
    int size = 100,
    CancellationToken cancellationToken = default)
{
    return await _commander.QueryAsync<User>(
        new { offset = (page - 1) * size, size },
        cancellationToken);
    //     Direct call ✅
}
```

### Overloads: Explicit Command Keys

Prefer distinct method names such as `RetrieveAllAsync` for list queries. If you choose to keep overloaded repository methods, pass explicit command keys directly on the `_commander` call:

```csharp
public async Task<User?> RetrieveAsync(int id, CancellationToken cancellationToken = default)
{
    return (await _commander.QueryAsync<User>(
        new { id },
        cancellationToken,
        method: "RetrieveAsync.ById")).FirstOrDefault();
}

public Task<IEnumerable<User>> RetrieveAsync(string email, CancellationToken cancellationToken = default)
{
    return _commander.QueryAsync<User>(
        new { email },
        cancellationToken,
        method: "RetrieveAsync.ByEmail");
}
```

---

## Caching Behavior

### First Call - Cache Miss

```
Request 1: UserRepository.RetrieveAsync(id: 1)
├─ Command cache check
│  └─ Key: "MyApp.Data.UserRepository.RetrieveAsync"
│  └─ Result: NOT FOUND (first call)
├─ Read settings
│  └─ Search: namespaces[0].types[0].commands["RetrieveAsync"]
│  └─ Result: CommandSetting found
└─ Cache insert
    └─ _commandCache["MyApp.Data.UserRepository.RetrieveAsync"] = setting
    └─ Cost: one-time settings traversal + cache write
```

### Second+ Calls - Cache Hit

```
Request 2: UserRepository.RetrieveAsync(id: 2)
├─ Command cache check
│  └─ Key: "MyApp.Data.UserRepository.RetrieveAsync"
│  └─ Result: FOUND in cache
└─ Use cached CommandSetting immediately
    └─ Cost: dictionary lookup only
```

### Cache Effectiveness

In a typical application:
- **First 30-50 unique method calls**: Cache miss (build cache)
- **Subsequent thousands of calls**: Cache hits (reuse)
- **Hit rate**: Typically high after warmup, depending on workload
- **Per-request overhead**: Negligible after warmup

---

## Multi-Mapping with Split Columns

For queries that join multiple tables and return multiple types:

```csharp
// Method signature
public async Task<IEnumerable<UserWithProfile>> RetrieveWithProfilesAsync(CancellationToken cancellationToken = default)
{
    return await _commander.QueryAsync<User, Profile, UserWithProfile>(
        (user, profile) => new UserWithProfile { User = user, Profile = profile },
        cancellationToken: cancellationToken
    );
}

// Configuration
{
    "RetrieveWithProfilesAsync": {
        "commandText": "SELECT * FROM Users u JOIN Profiles p ON u.Id = p.UserId",
        "connectionAlias": "Default",
        "split": "Id"  // ← Dapper uses this to split result rows
    }
}
```

The `split` column tells Dapper where to split the result set into separate types.

---

## Multiple Result Sets

For stored procedures returning multiple result sets:

```csharp
// Method
public async Task<(IEnumerable<User>, IEnumerable<Profile>)> RetrieveDataAsync(CancellationToken cancellationToken = default)
{
    return await _commander.QueryAsync<User, IEnumerable<Profile>, (IEnumerable<User>, IEnumerable<Profile>)>(
        (users, profiles) => (users, profiles),
        cancellationToken: cancellationToken
    );
}

// Configuration
{
    "RetrieveDataAsync": {
        "commandText": "EXEC sp_GetUserData",
        "commandType": "StoredProcedure",
        "connectionAlias": "Default"
    }
}
```

---

## Error Handling

### Command Not Found

```csharp
// Code:
public async Task<User?> RetrieveAsync(int id) { ... }

// Missing configuration:
{
    "commands": {
        // "RetrieveAsync" missing ✗
    }
}

// Result:
throw new InvalidOperationException(
    "Command 'MyApp.Data.UserRepository.RetrieveAsync' not found in configuration");
```

### Connection Alias Not Found

```csharp
// Configuration:
{
    "RetrieveAsync": {
        "commandText": "SELECT ...",
        "connectionAlias": "NonExistent"  // ✗ Not defined in connections[]
    }
}

// Result:
throw new InvalidOperationException(
    "Connection alias 'NonExistent' not found");
```

### Invalid Namespace/Type

If code namespace doesn't match configuration:

```csharp
// Code namespace: "MyApp.Data"
// Config namespace: "MyAppData"  (missing the dot)

// Result:
No matching configuration found; command lookup fails
```

---

## Performance Tips

1. **Prefer distinct method names for the best cache and docs clarity**:
    - Use names like `RetrieveAsync` and `RetrieveAllAsync`
    - Only use explicit keys per overload when overloads are unavoidable
    - Example keys: `RetrieveAsync.ById`, `RetrieveAsync.ByEmail`

2. **Reuse similar SQL patterns**:
   ```json
   {
       "RetrieveAllAsync": { "commandText": "SELECT ... ORDER BY Name OFFSET @offset ROWS FETCH NEXT @size ROWS ONLY" },
       "RetrieveAsync": { "commandText": "SELECT ... WHERE Id = @id" }
   }
   ```

3. **Use stored procedures for complex logic**:
   ```json
   {
       "RetrieveComplexReport": {
           "commandText": "sp_ComplexReport",
           "commandType": "StoredProcedure"
       }
   }
   ```

4. **Maximize cache hits**:
   - Warm up cache at startup (optional)
   - Query patterns naturally build cache over time

---

## Related Pages

- [Architecture Overview](index.md)
- [Configuration Overview](../configuration/index.md)
- [Code Structure](../code-structure.md)



