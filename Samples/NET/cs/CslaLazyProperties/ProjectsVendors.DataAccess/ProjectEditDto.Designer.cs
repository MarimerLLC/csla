using System;
using Csla;

namespace ProjectsVendors.DataAccess
{
    /// <summary>
    /// DTO for ProjectEdit type
    /// </summary>
    public partial class ProjectEditDto
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

        /// <summary>
        /// Gets or sets the Start Date.
        /// </summary>
        /// <value>The Start Date.</value>
        public SmartDate StartDate { get; set; }

        /// <summary>
        /// Gets or sets the Delivery Date.
        /// </summary>
        /// <value>The Delivery Date.</value>
        public SmartDate DeliveryDate { get; set; }
    }
}
