using System;
using System.Collections.Generic;
using Csla;

namespace ProjectsVendors.DataAccess
{
    /// <summary>
    /// DAL Interface for ProjectList type
    /// </summary>
    public partial interface IProjectListDal
    {
        /// <summary>
        /// Loads a ProjectList collection from the database.
        /// </summary>
        /// <returns>A list of <see cref="ProjectInfoDto"/>.</returns>
        List<ProjectInfoDto> Fetch();
    }
}
