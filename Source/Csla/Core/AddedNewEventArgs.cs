//-----------------------------------------------------------------------
// <copyright file="AddedNewEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Object containing information about a</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core
{
    /// <summary>
    /// Object containing information about a
    /// newly added object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the object that was added.
    /// </typeparam>
    public class AddedNewEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets a reference to the newly added
        /// object.
        /// </summary>
        public T NewObject { get; protected set; }

        /// <summary>
        /// Creates a new instance of the object.
        /// </summary>
        public AddedNewEventArgs() { }

        /// <summary>
        /// Creates a new instance of the object.
        /// </summary>
        /// <param name="newObject">
        /// The newly added object.
        /// </param>
        public AddedNewEventArgs(T newObject)
        {
            NewObject = newObject;
        }
    }
}