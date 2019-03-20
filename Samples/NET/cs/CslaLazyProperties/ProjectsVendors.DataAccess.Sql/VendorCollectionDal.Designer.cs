using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using ProjectsVendors.DataAccess;

namespace ProjectsVendors.DataAccess.Sql
{
    /// <summary>
    /// DAL SQL Server implementation of <see cref="IVendorCollectionDal"/>
    /// </summary>
    public partial class VendorCollectionDal : IVendorCollectionDal
    {

        #region DAL methods

        /// <summary>
        /// Loads a VendorCollection collection from the database.
        /// </summary>
        /// <param name="projectId">The fetch criteria.</param>
        /// <returns>A list of <see cref="VendorItemDto"/>.</returns>
        public List<VendorItemDto> Fetch(int projectId)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("ProjectsVendors"))
            {
                using (var cmd = new SqlCommand("dbo.GetVendorCollection", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectId", projectId).DbType = DbType.Int32;
                    var dr = cmd.ExecuteReader();
                    return LoadCollection(dr);
                }
            }
        }

        private List<VendorItemDto> LoadCollection(IDataReader data)
        {
            var vendorCollection = new List<VendorItemDto>();
            using (var dr = new SafeDataReader(data))
            {
                while (dr.Read())
                {
                    vendorCollection.Add(Fetch(dr));
                }
            }
            return vendorCollection;
        }

        private VendorItemDto Fetch(SafeDataReader dr)
        {
            var vendorItem = new VendorItemDto();
            // Value properties
            vendorItem.VendorId = dr.GetInt32("VendorId");
            vendorItem.VendorName = dr.GetString("VendorName");
            vendorItem.VendorContact = dr.GetString("VendorContact");
            vendorItem.VendorPhone = dr.GetString("VendorPhone");
            vendorItem.VendorEmail = dr.GetString("VendorEmail");
            vendorItem.IsPrimaryVendor = dr.GetBoolean("IsPrimaryVendor");
            vendorItem.LastUpdated = dr.GetSmartDate("LastUpdated", true);

            return vendorItem;
        }

        #endregion

    }
}
