using System;
using System.Collections.Generic;
using Csla;

namespace ProjectsVendors.DataAccess
{
    /// <summary>
    /// DAL Interface for VendorItem type
    /// </summary>
    public partial interface IVendorItemDal
    {
        /// <summary>
        /// Inserts a new VendorItem object in the database.
        /// </summary>
        /// <param name="vendorItem">The Vendor Item DTO.</param>
        /// <returns>The new <see cref="VendorItemDto"/>.</returns>
        VendorItemDto Insert(VendorItemDto vendorItem);

        /// <summary>
        /// Updates in the database all changes made to the VendorItem object.
        /// </summary>
        /// <param name="vendorItem">The Vendor Item DTO.</param>
        /// <returns>The updated <see cref="VendorItemDto"/>.</returns>
        VendorItemDto Update(VendorItemDto vendorItem);

        /// <summary>
        /// Deletes the VendorItem object from database.
        /// </summary>
        /// <param name="vendorId">The Vendor Id.</param>
        void Delete(int vendorId);
    }
}
