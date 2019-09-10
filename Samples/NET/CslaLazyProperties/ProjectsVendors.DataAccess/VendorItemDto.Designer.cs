using System;
using Csla;

namespace ProjectsVendors.DataAccess
{
    /// <summary>
    /// DTO for VendorItem type
    /// </summary>
    public partial class VendorItemDto
    {
        /// <summary>
        /// Gets or sets the parent Project Id.
        /// </summary>
        /// <value>The Project Id.</value>
        public int Parent_ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the Vendor Id.
        /// </summary>
        /// <value>The Vendor Id.</value>
        public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets the Vendor Name.
        /// </summary>
        /// <value>The Vendor Name.</value>
        public string VendorName { get; set; }

        /// <summary>
        /// Gets or sets the Vendor Contact.
        /// </summary>
        /// <value>The Vendor Contact.</value>
        public string VendorContact { get; set; }

        /// <summary>
        /// Gets or sets the Vendor Phone.
        /// </summary>
        /// <value>The Vendor Phone.</value>
        public string VendorPhone { get; set; }

        /// <summary>
        /// Gets or sets the Vendor Email.
        /// </summary>
        /// <value>The Vendor Email.</value>
        public string VendorEmail { get; set; }

        /// <summary>
        /// Gets or sets the Is Primary Vendor.
        /// </summary>
        /// <value><c>true</c> if Is Primary Vendor; otherwise, <c>false</c>.</value>
        public bool IsPrimaryVendor { get; set; }

        /// <summary>
        /// Gets or sets the Last Updated.
        /// </summary>
        /// <value>The Last Updated.</value>
        public SmartDate LastUpdated { get; set; }
    }
}
