//  ============================================================================================================================= 
//  author       : david sexton (@sextondjc | sextondjc.com)
//  date         : 2017.10.15 (17:58)
//  licence      : This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
//  =============================================================================================================================
namespace Syrx.Commanders.Databases
{
    /// <summary>
    /// Partial declaration of <see cref="DatabaseCommander{TRepository}"/> containing synchronous command execution APIs.
    /// </summary>
    /// <typeparam name="TRepository">The repository type whose methods are resolved to configured database commands.</typeparam>
    public sealed partial class DatabaseCommander<TRepository>
    {
    private static readonly string RepositoryTypeName = typeof(TRepository).FullName ?? typeof(TRepository).Name;

        /// <summary>
        /// Executes a command without parameters and returns a boolean indicating success.
        /// The command executed is automatically resolved based on the calling method name.
        /// </summary>
        /// <typeparam name="TResult">The type parameter (used for command resolution but not for return value).</typeparam>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns><c>true</c> if the command affected one or more rows; otherwise, <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during command execution.</exception>
        public bool Execute<TResult>([CallerMemberName] string method = null)
        {
            return ExecuteCore<TResult>(parameters: null, method);
        }

        /// <summary>
        /// Executes a command with the provided model as parameters and returns a boolean indicating success.
        /// The command executed is automatically resolved based on the calling method name.
        /// </summary>
        /// <typeparam name="TResult">The type of the model used as parameters.</typeparam>
        /// <param name="model">The model instance containing parameters for the command.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns><c>true</c> if the command affected one or more rows; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command setting cannot be found for the specified method.</exception>
        /// <exception cref="System.Data.SqlException">Thrown when a database-related error occurs during command execution.</exception>
        public bool Execute<TResult>(TResult model, [CallerMemberName] string method = null)
        {
            Throw<ArgumentNullException>(model != null, nameof(model));
            return ExecuteCore<TResult>(parameters: model!, method);
        }

        /// <summary>
        /// Core Execute implementation that handles all database execute operations with transaction management.
        /// This method is the central point for all database execute operations, eliminating code duplication.
        /// </summary>
        private bool ExecuteCore<TResult>(object parameters, string method)
        {
            var setting = GetCommandSetting(method);
            using (var connection = _connector.CreateConnection(setting))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(setting.IsolationLevel))
                {
                    try
                    {
                        var command = GetCommandDefinition(setting, parameters, transaction);
                        var result = (connection.Execute(command) > 0);
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            TraceExecuteRollbackFailure(rollbackEx, setting, method, isAsync: false);
                        }

                        TraceExecuteFailure(ex, setting, method, isAsync: false);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Executes a user-defined function within a transaction scope and returns the result.
        /// </summary>
        /// <typeparam name="TResult">The return type of the mapping function.</typeparam>
        /// <param name="map">A function to execute within the transaction scope.</param>
        /// <param name="scopeOption">The transaction scope option. Defaults to <see cref="TransactionScopeOption.Suppress"/>.</param>
        /// <param name="method">The name of the calling method. This parameter is automatically populated by the compiler.</param>
        /// <returns>The result of executing the <paramref name="map"/> function.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="map"/> is <c>null</c>.</exception>
        public TResult Execute<TResult>(Func<TResult> map,
            TransactionScopeOption scopeOption = TransactionScopeOption.Suppress,
            [CallerMemberName] string method = null)
        {
            Throw<ArgumentNullException>(map != null, nameof(map));

            // thought: should we support passing in the transaction scope option?
            //          i.e. let it be overridden by query definition?
            using (var scope = new TransactionScope(scopeOption))
            {
                var result = map();
                scope.Complete();
                return result;
            }
        }

        private static void TraceExecuteFailure(Exception ex, CommandSetting setting, string method, bool isAsync)
        {
            System.Diagnostics.Trace.TraceError(
                "event=database_execute_failure async={0} repository={1} method={2} alias={3} command_type={4} exception={5}",
                isAsync,
                RepositoryTypeName,
                method,
                setting?.ConnectionAlias,
                setting?.CommandType,
                ex.GetType().FullName);
        }

        private static void TraceExecuteRollbackFailure(Exception ex, CommandSetting setting, string method, bool isAsync)
        {
            System.Diagnostics.Trace.TraceError(
                "event=database_execute_rollback_failure async={0} repository={1} method={2} alias={3} command_type={4} exception={5}",
                isAsync,
                RepositoryTypeName,
                method,
                setting?.ConnectionAlias,
                setting?.CommandType,
                ex.GetType().FullName);
        }
    }
}