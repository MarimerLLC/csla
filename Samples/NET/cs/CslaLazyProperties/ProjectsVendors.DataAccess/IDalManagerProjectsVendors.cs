//-----------------------------------------------------------------------
// <copyright file="IDalManagerProjectsVendors.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace ProjectsVendors.DataAccess
{
    /// <summary>
    /// Defines the ProjectsVendors DAL manager interface for DAL providers.
    /// </summary>
    public interface IDalManagerProjectsVendors : IDisposable
    {
        /// <summary>
        /// Gets the DAL provider for a given object Type.
        /// </summary>
        /// <typeparam name="T">Object Type that requires a ProjectsVendors DAL provider.</typeparam>
        /// <returns>A new ProjectsVendors DAL instance for the given Type.</returns>
        T GetProvider<T>() where T : class;
    }
}
