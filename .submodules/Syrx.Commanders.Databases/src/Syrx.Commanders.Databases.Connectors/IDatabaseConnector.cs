//  ============================================================================================================================= 
//  author       : david sexton (@sextondjc | sextondjc.com)
//  date         : 2017.10.15 (17:58)
//  licence      : This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
//  =============================================================================================================================

namespace Syrx.Commanders.Databases.Connectors
{
    /// <summary>
    /// Represents a database connector capable of creating ADO.NET connections
    /// for the database commands used by the commander layer.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface are responsible for resolving the
    /// correct <see cref="IDbConnection"/> instance for a given
    /// <see cref="CommandSetting"/>. Typical implementations obtain a
    /// provider factory and create a provider-specific connection instance.
    /// </remarks>
    public interface IDatabaseConnector : IConnector<IDbConnection, CommandSetting>
    {
    }
}

