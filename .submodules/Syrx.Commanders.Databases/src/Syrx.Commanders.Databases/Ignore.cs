//  ============================================================================================================================= 
//  author       : david sexton (@sextondjc | sextondjc.com)
//  date         : 2017.10.15 (17:58)
//  licence      : This file is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package.
//  =============================================================================================================================

namespace Syrx.Commanders.Databases
{
    /// <summary>
    /// A marker type used to indicate unused generic type parameters in method delegation patterns.
    /// This provides explicit intent that certain type parameters should be ignored during type analysis.
    /// </summary>
    internal readonly struct Ignore
    {
        // Empty struct - used purely as a type marker
    }
}