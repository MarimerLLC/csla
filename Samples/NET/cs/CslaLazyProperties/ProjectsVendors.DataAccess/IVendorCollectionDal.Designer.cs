using System;
using System.Collections.Generic;
using Csla;

namespace ProjectsVendors.DataAccess
{
    /// <summary>
    /// DAL Interface for VendorCollection type
    /// </summary>
    public partial interface IVendorCollectionDal
    {
        /// <summary>
        /// Loads a VendorCollection collection from the database.
        /// </summary>
        /// <param name="projectId">The fetch criteria.</param>
        /// <returns>A list of <see cref="VendorItemDto"/>.</returns>
        List<VendorItemDto> Fetch(int projectId);
    }
}
