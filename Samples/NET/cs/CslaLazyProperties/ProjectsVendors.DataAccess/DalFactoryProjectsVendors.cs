//-----------------------------------------------------------------------
// <copyright file="DalFactoryProjectsVendors.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Configuration;

namespace ProjectsVendors.DataAccess
{
    /// <summary>
    /// Creates a ProjectsVendors DAL manager provider.
    /// </summary>
    /// <remarks>
    /// To use the generated DAL:<br/>
    /// 1) name this assembly ProjectsVendors.DataAccess<br/>
    /// 2) add the following line to the <strong>appSettings</strong>
    /// section of the application .config file: <br/>
    /// &lt;add key="ProjectsVendors.DalManagerType" value="ProjectsVendors.DataAccess.Sql.DalManagerProjectsVendors, ProjectsVendors.DataAccess.Sql" /&gt;
    /// </remarks>
    public static class DalFactoryProjectsVendors
    {
        private static Type _dalType;

        /// <summary>Gets the ProjectsVendors DAL manager type that must be set
        /// in the <strong>appSettings</strong> section of the application .config file.</summary>
        /// <returns>A new <see cref="IDalManagerProjectsVendors"/> instance</returns>
        public static IDalManagerProjectsVendors GetManager()
        {
            if (_dalType == null)
            {
                var dalTypeName = ConfigurationManager.AppSettings["ProjectsVendors.DalManagerType"];
                if (!string.IsNullOrEmpty(dalTypeName))
                    _dalType = Type.GetType(dalTypeName);
                else
                    throw new NullReferenceException("ProjectsVendors.DalManagerType");
                if (_dalType == null)
                    throw new ArgumentException(string.Format("Type {0} could not be found", dalTypeName));
            }
            return (IDalManagerProjectsVendors) Activator.CreateInstance(_dalType);
        }
    }
}
