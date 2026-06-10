namespace Syrx.Commanders.Databases.Settings
{
    /// <summary>
    /// Defines flags that control how Dapper executes configured commands.
    /// </summary>
    [Flags]
    public enum CommandFlagSetting
    {
        /// <summary>
        /// Applies no special behavior.
        /// </summary>
        None = 0,

        /// <summary>
        /// Buffers the entire result set in memory before returning it.
        /// </summary>
        Buffered = 1,

        /// <summary>
        /// Enables pipelined command execution when supported by the provider.
        /// </summary>
        Pipelined = 2,

        /// <summary>
        /// Bypasses Dapper's command plan cache for the execution.
        /// </summary>
        NoCache = 4
    }
}
