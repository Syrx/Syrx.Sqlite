//  ============================================================================================================================= 
//  author       : david sexton (@sextondjc | sextondjc.com)
//  date         : 2017.10.15 (17:58)
//  licence      : This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
//  =============================================================================================================================

namespace Syrx.Commanders.Databases.Settings.Readers
{
    /// <summary>
    /// Defines a reader for database command settings. This interface is used internally
    /// by <see cref="DatabaseCommander{TRepository}"/> to resolve configured commands
    /// based on the repository type and method name.
    /// </summary>
    /// <remarks>
    /// This interface is primarily an internal contract and should not typically be
    /// implemented directly by application code. The framework provides standard
    /// implementations that work with <see cref="ICommanderSettings"/>.
    /// </remarks>
    public interface IDatabaseCommandReader : ICommandReader<CommandSetting> { }
}