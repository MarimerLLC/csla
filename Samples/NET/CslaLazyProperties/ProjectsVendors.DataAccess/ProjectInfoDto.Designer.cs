using System;
using Csla;

namespace ProjectsVendors.DataAccess
{
    /// <summary>
    /// DTO for ProjectInfo type
    /// </summary>
    public partial class ProjectInfoDto
    {
        /// <summary>
        /// Gets or sets the Project Id.
        /// </summary>
        /// <value>The Project Id.</value>
        public int ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the Project Name.
        /// </summary>
        /// <value>The Project Name.</value>
        public string ProjectName { get; set; }
    }
}
