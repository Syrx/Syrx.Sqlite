//  ============================================================================================================================= 
//  author       : david sexton (@sextondjc | sextondjc.com)
//  date         : 2017.10.15 (17:58)
//  licence      : This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
//  =============================================================================================================================
namespace Syrx.Commanders.Databases
{
    /// <summary>
    /// Partial declaration of <see cref="DatabaseCommander{TRepository}"/> containing synchronous multi-mapping query APIs.
    /// </summary>
    /// <typeparam name="TRepository">The repository type whose methods are resolved to configured database commands.</typeparam>
    public sealed partial class DatabaseCommander<TRepository>
    {
        /// <summary>
        /// Executes a query that returns a sequence of data of type <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of objects to return from the query.</typeparam>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<TResult>(
            object parameters = null,
            [CallerMemberName] string method = null)
        {
            var setting = GetCommandSetting(method);
            var command = GetCommandDefinition(setting, parameters);
            using var connection = _connector.CreateConnection(setting);
            return connection.Query<TResult>(command);
        }

        /// <summary>
        /// Executes a multimap query that combines data from two result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the two input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, TResult>(
            Func<T1, T2, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) => Query<T1, T2, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from three result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the three input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, TResult>(
            Func<T1, T2, T3, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) => Query<T1, T2, T3, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from four result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the four input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) => Query<T1, T2, T3, T4, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from five result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="T5">The type of the fifth object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the five input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, TResult>(
            Func<T1, T2, T3, T4, T5, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) => Query<T1, T2, T3, T4, T5, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from six result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="T5">The type of the fifth object in the result set.</typeparam>
        /// <typeparam name="T6">The type of the sixth object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the six input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, T6, TResult>(
            Func<T1, T2, T3, T4, T5, T6, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) =>
            Query<T1, T2, T3, T4, T5, T6, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from seven result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="T5">The type of the fifth object in the result set.</typeparam>
        /// <typeparam name="T6">The type of the sixth object in the result set.</typeparam>
        /// <typeparam name="T7">The type of the seventh object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the seven input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, T6, T7, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) =>
            Query<T1, T2, T3, T4, T5, T6, T7, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7),
                parameters,
                method);


        /// <summary>
        /// Executes a multimap query that combines data from eight result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="T5">The type of the fifth object in the result set.</typeparam>
        /// <typeparam name="T6">The type of the sixth object in the result set.</typeparam>
        /// <typeparam name="T7">The type of the seventh object in the result set.</typeparam>
        /// <typeparam name="T8">The type of the eighth object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the eight input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) =>
            Query<T1, T2, T3, T4, T5, T6, T7, T8, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from nine result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="T5">The type of the fifth object in the result set.</typeparam>
        /// <typeparam name="T6">The type of the sixth object in the result set.</typeparam>
        /// <typeparam name="T7">The type of the seventh object in the result set.</typeparam>
        /// <typeparam name="T8">The type of the eighth object in the result set.</typeparam>
        /// <typeparam name="T9">The type of the ninth object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the nine input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) =>
            Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from ten result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="T5">The type of the fifth object in the result set.</typeparam>
        /// <typeparam name="T6">The type of the sixth object in the result set.</typeparam>
        /// <typeparam name="T7">The type of the seventh object in the result set.</typeparam>
        /// <typeparam name="T8">The type of the eighth object in the result set.</typeparam>
        /// <typeparam name="T9">The type of the ninth object in the result set.</typeparam>
        /// <typeparam name="T10">The type of the tenth object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the ten input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) =>
            Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Ignore, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from eleven result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="T5">The type of the fifth object in the result set.</typeparam>
        /// <typeparam name="T6">The type of the sixth object in the result set.</typeparam>
        /// <typeparam name="T7">The type of the seventh object in the result set.</typeparam>
        /// <typeparam name="T8">The type of the eighth object in the result set.</typeparam>
        /// <typeparam name="T9">The type of the ninth object in the result set.</typeparam>
        /// <typeparam name="T10">The type of the tenth object in the result set.</typeparam>
        /// <typeparam name="T11">The type of the eleventh object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the eleven input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) =>
            Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Ignore, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from twelve result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="T5">The type of the fifth object in the result set.</typeparam>
        /// <typeparam name="T6">The type of the sixth object in the result set.</typeparam>
        /// <typeparam name="T7">The type of the seventh object in the result set.</typeparam>
        /// <typeparam name="T8">The type of the eighth object in the result set.</typeparam>
        /// <typeparam name="T9">The type of the ninth object in the result set.</typeparam>
        /// <typeparam name="T10">The type of the tenth object in the result set.</typeparam>
        /// <typeparam name="T11">The type of the eleventh object in the result set.</typeparam>
        /// <typeparam name="T12">The type of the twelfth object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the twelve input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) =>
            Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Ignore, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from thirteen result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="T5">The type of the fifth object in the result set.</typeparam>
        /// <typeparam name="T6">The type of the sixth object in the result set.</typeparam>
        /// <typeparam name="T7">The type of the seventh object in the result set.</typeparam>
        /// <typeparam name="T8">The type of the eighth object in the result set.</typeparam>
        /// <typeparam name="T9">The type of the ninth object in the result set.</typeparam>
        /// <typeparam name="T10">The type of the tenth object in the result set.</typeparam>
        /// <typeparam name="T11">The type of the eleventh object in the result set.</typeparam>
        /// <typeparam name="T12">The type of the twelfth object in the result set.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the thirteen input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) =>
            Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Ignore, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from fourteen result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="T5">The type of the fifth object in the result set.</typeparam>
        /// <typeparam name="T6">The type of the sixth object in the result set.</typeparam>
        /// <typeparam name="T7">The type of the seventh object in the result set.</typeparam>
        /// <typeparam name="T8">The type of the eighth object in the result set.</typeparam>
        /// <typeparam name="T9">The type of the ninth object in the result set.</typeparam>
        /// <typeparam name="T10">The type of the tenth object in the result set.</typeparam>
        /// <typeparam name="T11">The type of the eleventh object in the result set.</typeparam>
        /// <typeparam name="T12">The type of the twelfth object in the result set.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth object in the result set.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the fourteen input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) =>
            Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Ignore, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from fifteen result sets using the specified mapping function.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="T5">The type of the fifth object in the result set.</typeparam>
        /// <typeparam name="T6">The type of the sixth object in the result set.</typeparam>
        /// <typeparam name="T7">The type of the seventh object in the result set.</typeparam>
        /// <typeparam name="T8">The type of the eighth object in the result set.</typeparam>
        /// <typeparam name="T9">The type of the ninth object in the result set.</typeparam>
        /// <typeparam name="T10">The type of the tenth object in the result set.</typeparam>
        /// <typeparam name="T11">The type of the eleventh object in the result set.</typeparam>
        /// <typeparam name="T12">The type of the twelfth object in the result set.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth object in the result set.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth object in the result set.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the fifteen input objects to the result object.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null) =>
            Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Ignore, TResult>(
                (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16) => map(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15),
                parameters,
                method);

        /// <summary>
        /// Executes a multimap query that combines data from up to sixteen result sets using the specified mapping function.
        /// This is the core implementation method that all other multimap query overloads delegate to.
        /// </summary>
        /// <typeparam name="T1">The type of the first object in the result set.</typeparam>
        /// <typeparam name="T2">The type of the second object in the result set.</typeparam>
        /// <typeparam name="T3">The type of the third object in the result set.</typeparam>
        /// <typeparam name="T4">The type of the fourth object in the result set.</typeparam>
        /// <typeparam name="T5">The type of the fifth object in the result set.</typeparam>
        /// <typeparam name="T6">The type of the sixth object in the result set.</typeparam>
        /// <typeparam name="T7">The type of the seventh object in the result set.</typeparam>
        /// <typeparam name="T8">The type of the eighth object in the result set.</typeparam>
        /// <typeparam name="T9">The type of the ninth object in the result set.</typeparam>
        /// <typeparam name="T10">The type of the tenth object in the result set.</typeparam>
        /// <typeparam name="T11">The type of the eleventh object in the result set.</typeparam>
        /// <typeparam name="T12">The type of the twelfth object in the result set.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth object in the result set.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth object in the result set.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth object in the result set.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth object in the result set.</typeparam>
        /// <typeparam name="TResult">The return type after applying the mapping function.</typeparam>
        /// <param name="map">A function to map the input objects to the result object. Unused parameters will be set to their default values.</param>
        /// <param name="parameters">The parameters to pass to the command. Can be null if no parameters are required.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>A sequence of data of type <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during query execution.</exception>
        /// <remarks>
        /// This method automatically detects the number of types to be mapped by identifying <see cref="Ignore"/> placeholder types.
        /// When <see cref="Ignore"/> types are encountered, they are excluded from the type array passed to the underlying Dapper query.
        /// Cached generic type-shape metadata avoids rebuilding the type array on each call.
        /// </remarks>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> map,
            object parameters = null,
            [CallerMemberName] string method = null)
        {
            var setting = GetCommandSetting(method);
            var command = GetCommandDefinition(setting, parameters);
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
            return connection.Query(
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