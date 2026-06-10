//  ============================================================================================================================= 
//  author       : david sexton (@sextondjc | sextondjc.com)
//  date         : 2017.10.15 (17:58)
//  licence      : This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
//  =============================================================================================================================


namespace Syrx.Commanders.Databases
{
    /// <summary>
    /// Partial declaration of <see cref="DatabaseCommander{TRepository}"/> containing asynchronous multi-mapping query APIs.
    /// </summary>
    /// <typeparam name="TRepository">The repository type whose methods are resolved to configured database commands.</typeparam>
    public sealed partial class DatabaseCommander<TRepository> //: ICommander
    {
        private static class MultimapTypeShape<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
        {
            public static readonly int Count = GetCount();
            public static readonly Type[] Types = GetTypes();

            private static int GetCount()
            {
                if (typeof(T1) == typeof(Ignore)) return 0;
                if (typeof(T2) == typeof(Ignore)) return 1;
                if (typeof(T3) == typeof(Ignore)) return 2;
                if (typeof(T4) == typeof(Ignore)) return 3;
                if (typeof(T5) == typeof(Ignore)) return 4;
                if (typeof(T6) == typeof(Ignore)) return 5;
                if (typeof(T7) == typeof(Ignore)) return 6;
                if (typeof(T8) == typeof(Ignore)) return 7;
                if (typeof(T9) == typeof(Ignore)) return 8;
                if (typeof(T10) == typeof(Ignore)) return 9;
                if (typeof(T11) == typeof(Ignore)) return 10;
                if (typeof(T12) == typeof(Ignore)) return 11;
                if (typeof(T13) == typeof(Ignore)) return 12;
                if (typeof(T14) == typeof(Ignore)) return 13;
                if (typeof(T15) == typeof(Ignore)) return 14;
                if (typeof(T16) == typeof(Ignore)) return 15;

                return 16;
            }

            private static Type[] GetTypes()
                => Count switch
                {
                    0 => [],
                    1 => [typeof(T1)],
                    2 => [typeof(T1), typeof(T2)],
                    3 => [typeof(T1), typeof(T2), typeof(T3)],
                    4 => [typeof(T1), typeof(T2), typeof(T3), typeof(T4)],
                    5 => [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)],
                    6 => [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)],
                    7 => [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)],
                    8 => [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)],
                    9 => [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9)],
                    10 => [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10)],
                    11 => [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11)],
                    12 => [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12)],
                    13 => [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13)],
                    14 => [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14)],
                    15 => [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15)],
                    _ => [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16)]
                };
        }

        /// <summary>
        /// Asynchronously executes a query against the database and returns a sequence of objects of type <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of objects to return from the query.</typeparam>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the method parameter resolves to a null command setting.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<TResult>(object parameters = null,
            CancellationToken cancellationToken = default, [CallerMemberName] string method = null)
        {
            var setting = GetCommandSetting(method);
            var command = GetCommandDefinition(setting, parameters, cancellationToken: cancellationToken);
            using var connection = _connector.CreateConnection(setting);
            return await connection.QueryAsync<TResult>(command);
        }

        /// <summary>
        /// Asynchronously executes a multimap query using two types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of <typeparamref name="T1"/> and <typeparamref name="T2"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, TResult>(Func<T1, T2, TResult> map,
            object parameters = null, CancellationToken cancellationToken = default,
            [CallerMemberName] string method = null)
        {
            var setting = GetCommandSetting(method);
            var command = GetCommandDefinition(setting, parameters, cancellationToken: cancellationToken);
            using var connection = _connector.CreateConnection(setting);
            return await connection.QueryAsync(command, map, setting.Split);
        }

        /// <summary>
        /// Asynchronously executes a multimap query using three types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of <typeparamref name="T1"/>, <typeparamref name="T2"/>, and <typeparamref name="T3"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> map,
            object parameters = null, CancellationToken cancellationToken = default,
            [CallerMemberName] string method = null)
        {
            var setting = GetCommandSetting(method);
            var command = GetCommandDefinition(setting, parameters, cancellationToken: cancellationToken);
            using var connection = _connector.CreateConnection(setting);
            return await connection.QueryAsync(command, map, setting.Split);
        }

        /// <summary>
        /// Asynchronously executes a multimap query using four types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, and <typeparamref name="T4"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> map,
            object parameters = null, CancellationToken cancellationToken = default,
            [CallerMemberName] string method = null)
        {
            var setting = GetCommandSetting(method);
            var command = GetCommandDefinition(setting, parameters, cancellationToken: cancellationToken);
            using var connection = _connector.CreateConnection(setting);
            return await connection.QueryAsync(command, map, setting.Split);
        }

        /// <summary>
        /// Asynchronously executes a multimap query using five types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="T5">The type of the fifth object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of the five input types to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, T5, TResult>(
            Func<T1, T2, T3, T4, T5, TResult> map, object parameters = null,
            CancellationToken cancellationToken = default, [CallerMemberName] string method = null)
        {
            var setting = GetCommandSetting(method);
            var command = GetCommandDefinition(setting, parameters, cancellationToken: cancellationToken);
            using var connection = _connector.CreateConnection(setting);
            return await connection.QueryAsync(command, map, setting.Split);
        }

        /// <summary>
        /// Asynchronously executes a multimap query using six types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="T5">The type of the fifth object to map from the query results.</typeparam>
        /// <typeparam name="T6">The type of the sixth object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of the six input types to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, T5, T6, TResult>(
            Func<T1, T2, T3, T4, T5, T6, TResult> map, object parameters = null,
            CancellationToken cancellationToken = default, [CallerMemberName] string method = null)
        {
            var setting = GetCommandSetting(method);
            var command = GetCommandDefinition(setting, parameters, cancellationToken: cancellationToken);
            using var connection = _connector.CreateConnection(setting);
            return await connection.QueryAsync(command, map, setting.Split);
        }

        /// <summary>
        /// Asynchronously executes a multimap query using seven types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="T5">The type of the fifth object to map from the query results.</typeparam>
        /// <typeparam name="T6">The type of the sixth object to map from the query results.</typeparam>
        /// <typeparam name="T7">The type of the seventh object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of the seven input types to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, TResult> map, object parameters = null,
            CancellationToken cancellationToken = default, [CallerMemberName] string method = null)
        {
            var setting = GetCommandSetting(method);
            var command = GetCommandDefinition(setting, parameters, cancellationToken: cancellationToken);
            using var connection = _connector.CreateConnection(setting);
            return await connection.QueryAsync(command, map, setting.Split);
        }

        /// <summary>
        /// Asynchronously executes a multimap query using eight types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="T5">The type of the fifth object to map from the query results.</typeparam>
        /// <typeparam name="T6">The type of the sixth object to map from the query results.</typeparam>
        /// <typeparam name="T7">The type of the seventh object to map from the query results.</typeparam>
        /// <typeparam name="T8">The type of the eighth object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of the eight input types to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> map,
            object parameters = null,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string method = null)
            => await QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8),
                parameters,
                cancellationToken,
                method);

        /// <summary>
        /// Asynchronously executes a multimap query using nine types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="T5">The type of the fifth object to map from the query results.</typeparam>
        /// <typeparam name="T6">The type of the sixth object to map from the query results.</typeparam>
        /// <typeparam name="T7">The type of the seventh object to map from the query results.</typeparam>
        /// <typeparam name="T8">The type of the eighth object to map from the query results.</typeparam>
        /// <typeparam name="T9">The type of the ninth object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of the nine input types to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> map,
            object parameters = null,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string method = null)
            => await QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9),
                parameters,
                cancellationToken,
                method);

        /// <summary>
        /// Asynchronously executes a multimap query using ten types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="T5">The type of the fifth object to map from the query results.</typeparam>
        /// <typeparam name="T6">The type of the sixth object to map from the query results.</typeparam>
        /// <typeparam name="T7">The type of the seventh object to map from the query results.</typeparam>
        /// <typeparam name="T8">The type of the eighth object to map from the query results.</typeparam>
        /// <typeparam name="T9">The type of the ninth object to map from the query results.</typeparam>
        /// <typeparam name="T10">The type of the tenth object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of the ten input types to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> map,
            object parameters = null,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string method = null)
            => await QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10),
                parameters,
                cancellationToken,
                method);

        /// <summary>
        /// Asynchronously executes a multimap query using eleven types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="T5">The type of the fifth object to map from the query results.</typeparam>
        /// <typeparam name="T6">The type of the sixth object to map from the query results.</typeparam>
        /// <typeparam name="T7">The type of the seventh object to map from the query results.</typeparam>
        /// <typeparam name="T8">The type of the eighth object to map from the query results.</typeparam>
        /// <typeparam name="T9">The type of the ninth object to map from the query results.</typeparam>
        /// <typeparam name="T10">The type of the tenth object to map from the query results.</typeparam>
        /// <typeparam name="T11">The type of the eleventh object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of the eleven input types to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> map,
            object parameters = null,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string method = null)
            => await QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11),
                parameters,
                cancellationToken,
                method);

        /// <summary>
        /// Asynchronously executes a multimap query using twelve types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="T5">The type of the fifth object to map from the query results.</typeparam>
        /// <typeparam name="T6">The type of the sixth object to map from the query results.</typeparam>
        /// <typeparam name="T7">The type of the seventh object to map from the query results.</typeparam>
        /// <typeparam name="T8">The type of the eighth object to map from the query results.</typeparam>
        /// <typeparam name="T9">The type of the ninth object to map from the query results.</typeparam>
        /// <typeparam name="T10">The type of the tenth object to map from the query results.</typeparam>
        /// <typeparam name="T11">The type of the eleventh object to map from the query results.</typeparam>
        /// <typeparam name="T12">The type of the twelfth object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of the twelve input types to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> map,
            object parameters = null,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string method = null)
            => await QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12),
                parameters,
                cancellationToken,
                method);

        /// <summary>
        /// Asynchronously executes a multimap query using thirteen types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="T5">The type of the fifth object to map from the query results.</typeparam>
        /// <typeparam name="T6">The type of the sixth object to map from the query results.</typeparam>
        /// <typeparam name="T7">The type of the seventh object to map from the query results.</typeparam>
        /// <typeparam name="T8">The type of the eighth object to map from the query results.</typeparam>
        /// <typeparam name="T9">The type of the ninth object to map from the query results.</typeparam>
        /// <typeparam name="T10">The type of the tenth object to map from the query results.</typeparam>
        /// <typeparam name="T11">The type of the eleventh object to map from the query results.</typeparam>
        /// <typeparam name="T12">The type of the twelfth object to map from the query results.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of the thirteen input types to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> map,
            object parameters = null,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string method = null)
            => await QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13),
                parameters,
                cancellationToken,
                method);

        /// <summary>
        /// Asynchronously executes a multimap query using fourteen types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="T5">The type of the fifth object to map from the query results.</typeparam>
        /// <typeparam name="T6">The type of the sixth object to map from the query results.</typeparam>
        /// <typeparam name="T7">The type of the seventh object to map from the query results.</typeparam>
        /// <typeparam name="T8">The type of the eighth object to map from the query results.</typeparam>
        /// <typeparam name="T9">The type of the ninth object to map from the query results.</typeparam>
        /// <typeparam name="T10">The type of the tenth object to map from the query results.</typeparam>
        /// <typeparam name="T11">The type of the eleventh object to map from the query results.</typeparam>
        /// <typeparam name="T12">The type of the twelfth object to map from the query results.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth object to map from the query results.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of the fourteen input types to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> map,
            object parameters = null,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string method = null)
            => await QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14),
                parameters,
                cancellationToken,
                method);

        /// <summary>
        /// Asynchronously executes a multimap query using fifteen types, combining them with a mapping function to produce the result.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="T5">The type of the fifth object to map from the query results.</typeparam>
        /// <typeparam name="T6">The type of the sixth object to map from the query results.</typeparam>
        /// <typeparam name="T7">The type of the seventh object to map from the query results.</typeparam>
        /// <typeparam name="T8">The type of the eighth object to map from the query results.</typeparam>
        /// <typeparam name="T9">The type of the ninth object to map from the query results.</typeparam>
        /// <typeparam name="T10">The type of the tenth object to map from the query results.</typeparam>
        /// <typeparam name="T11">The type of the eleventh object to map from the query results.</typeparam>
        /// <typeparam name="T12">The type of the twelfth object to map from the query results.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth object to map from the query results.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth object to map from the query results.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of the fifteen input types to <typeparamref name="TResult"/>.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> map,
            object parameters = null,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string method = null)
            => await QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15),
                parameters,
                cancellationToken,
                method);


        /// <summary>
        /// Asynchronously executes a multimap query using up to sixteen types, combining them with a mapping function to produce the result.
        /// This is the core implementation method that all other async multimap query overloads delegate to.
        /// </summary>
        /// <typeparam name="T1">The type of the first object to map from the query results.</typeparam>
        /// <typeparam name="T2">The type of the second object to map from the query results.</typeparam>
        /// <typeparam name="T3">The type of the third object to map from the query results.</typeparam>
        /// <typeparam name="T4">The type of the fourth object to map from the query results.</typeparam>
        /// <typeparam name="T5">The type of the fifth object to map from the query results.</typeparam>
        /// <typeparam name="T6">The type of the sixth object to map from the query results.</typeparam>
        /// <typeparam name="T7">The type of the seventh object to map from the query results.</typeparam>
        /// <typeparam name="T8">The type of the eighth object to map from the query results.</typeparam>
        /// <typeparam name="T9">The type of the ninth object to map from the query results.</typeparam>
        /// <typeparam name="T10">The type of the tenth object to map from the query results.</typeparam>
        /// <typeparam name="T11">The type of the eleventh object to map from the query results.</typeparam>
        /// <typeparam name="T12">The type of the twelfth object to map from the query results.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth object to map from the query results.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth object to map from the query results.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth object to map from the query results.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth object to map from the query results.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function that maps instances of the input types to <typeparamref name="TResult"/>. Unused types will receive default values.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the cancellation token.</exception>
        /// <remarks>
        /// This method uses Dapper's multimap functionality to join multiple objects from a query result.
        /// It automatically detects the number of actual types by identifying <see cref="Ignore"/> placeholder types.
        /// For type parameters marked as <see cref="Ignore"/>, default values of the appropriate type are provided to the mapping function.
        /// The method uses cached generic type-shape metadata to avoid rebuilding the Dapper type array on each call.
        /// </remarks>
        public async Task<IEnumerable<TResult>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> map,
            object parameters = null,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string method = null)
        {
            var setting = GetCommandSetting(method);
            var command = GetCommandDefinition(setting, parameters, cancellationToken: cancellationToken);
            var count = MultimapTypeShape<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Count;
            var types = MultimapTypeShape<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>.Types;

            Func<object[], TResult> internalMapper = (a) =>
            {
                var one = count > 0 ? (T1)a[0] : default(T1)!;
                var two = count > 1 ? (T2)a[1] : default(T2)!;
                var three = count > 2 ? (T3)a[2] : default(T3)!;
                var four = count > 3 ? (T4)a[3] : default(T4)!;
                var five = count > 4 ? (T5)a[4] : default(T5)!;
                var six = count > 5 ? (T6)a[5] : default(T6)!;
                var seven = count > 6 ? (T7)a[6] : default(T7)!;
                var eight = count > 7 ? (T8)a[7] : default(T8)!;
                var nine = count > 8 ? (T9)a[8] : default(T9)!;
                var ten = count > 9 ? (T10)a[9] : default(T10)!;
                var eleven = count > 10 ? (T11)a[10] : default(T11)!;
                var twelve = count > 11 ? (T12)a[11] : default(T12)!;
                var thirteen = count > 12 ? (T13)a[12] : default(T13)!;
                var fourteen = count > 13 ? (T14)a[13] : default(T14)!;
                var fifteen = count > 14 ? (T15)a[14] : default(T15)!;
                var sixteen = count > 15 ? (T16)a[15] : default(T16)!;

                return map(one, two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen, fourteen, fifteen, sixteen);
            };

            using var connection = _connector.CreateConnection(setting);
            return await connection.QueryAsync(
                sql: command.CommandText,
                types: types,
                map: internalMapper,
                param: parameters,
                buffered: command.Buffered,
                splitOn: setting.Split,
                commandTimeout: command.CommandTimeout,
                commandType: setting.CommandType);
        }

    }
}
