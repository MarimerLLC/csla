using System;
using System.Collections.Generic;
using Csla;

namespace ProjectsVendors.DataAccess
{
    /// <summary>
    /// DAL Interface for ProjectEdit type
    /// </summary>
    public partial interface IProjectEditDal
    {
        /// <summary>
        /// Loads a ProjectEdit object from the database.
        /// </summary>
        /// <param name="projectId">The fetch criteria.</param>
        /// <returns>A <see cref="ProjectEditDto"/> object.</returns>
        ProjectEditDto Fetch(int projectId);

        /// <summary>
        /// Inserts a new ProjectEdit object in the database.
        /// </summary>
        /// <param name="projectEdit">The Project Edit DTO.</param>
        /// <returns>The new <see cref="ProjectEditDto"/>.</returns>
        ProjectEditDto Insert(ProjectEditDto projectEdit);

        /// <summary>
        /// Updates in the database all changes made to the ProjectEdit object.
        /// </summary>
        /// <param name="projectEdit">The Project Edit DTO.</param>
        /// <returns>The updated <see cref="ProjectEditDto"/>.</returns>
        ProjectEditDto Update(ProjectEditDto projectEdit);

        /// <summary>
        /// Deletes the ProjectEdit object from database.
        /// </summary>
        /// <param name="projectId">The delete criteria.</param>
        void Delete(int projectId);
    }
}
