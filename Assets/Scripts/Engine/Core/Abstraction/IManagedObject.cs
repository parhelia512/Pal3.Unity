// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2023, Jiaqi Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Engine.Core.Abstraction
{
    /// <summary>
    /// Interface for managed objects that wrap native objects.
    /// </summary>
    public interface IManagedObject
    {
        /// <summary>
        /// Gets the underlying native object.
        /// </summary>
        object NativeObject { get; }

        /// <summary>
        /// Gets a value indicating whether the underlying native object has been disposed.
        /// </summary>
        bool IsNativeObjectDisposed { get; }
    }
}