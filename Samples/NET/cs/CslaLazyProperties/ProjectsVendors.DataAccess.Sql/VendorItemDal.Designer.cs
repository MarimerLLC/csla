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
    /// DAL SQL Server implementation of <see cref="IVendorItemDal"/>
    /// </summary>
    public partial class VendorItemDal : IVendorItemDal
    {

        #region DAL methods

        /// <summary>
        /// Inserts a new VendorItem object in the database.
        /// </summary>
        /// <param name="vendorItem">The Vendor Item DTO.</param>
        /// <returns>The new <see cref="VendorItemDto"/>.</returns>
        public VendorItemDto Insert(VendorItemDto vendorItem)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("ProjectsVendors"))
            {
                using (var cmd = new SqlCommand("dbo.AddVendorItem", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectId", vendorItem.Parent_ProjectId).DbType = DbType.Int32;
                    cmd.Parameters.AddWithValue("@VendorId", vendorItem.VendorId).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@VendorName", vendorItem.VendorName).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@VendorContact", vendorItem.VendorContact).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@VendorPhone", vendorItem.VendorPhone).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@VendorEmail", vendorItem.VendorEmail).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@IsPrimaryVendor", vendorItem.IsPrimaryVendor).DbType = DbType.Boolean;
                    cmd.Parameters.AddWithValue("@LastUpdated", vendorItem.LastUpdated.DBValue).DbType = DbType.DateTime2;
                    cmd.ExecuteNonQuery();
                    vendorItem.VendorId = (int)cmd.Parameters["@VendorId"].Value;
                }
            }
            return vendorItem;
        }

        /// <summary>
        /// Updates in the database all changes made to the VendorItem object.
        /// </summary>
        /// <param name="vendorItem">The Vendor Item DTO.</param>
        /// <returns>The updated <see cref="VendorItemDto"/>.</returns>
        public VendorItemDto Update(VendorItemDto vendorItem)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("ProjectsVendors"))
            {
                using (var cmd = new SqlCommand("dbo.UpdateVendorItem", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VendorId", vendorItem.VendorId).DbType = DbType.Int32;
                    cmd.Parameters.AddWithValue("@VendorName", vendorItem.VendorName).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@VendorContact", vendorItem.VendorContact).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@VendorPhone", vendorItem.VendorPhone).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@VendorEmail", vendorItem.VendorEmail).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@IsPrimaryVendor", vendorItem.IsPrimaryVendor).DbType = DbType.Boolean;
                    cmd.Parameters.AddWithValue("@LastUpdated", vendorItem.LastUpdated.DBValue).DbType = DbType.DateTime2;
                    var rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        throw new DataNotFoundException("VendorItem");
                }
            }
            return vendorItem;
        }

        /// <summary>
        /// Deletes the VendorItem object from database.
        /// </summary>
        /// <param name="vendorId">The Vendor Id.</param>
        public void Delete(int vendorId)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("ProjectsVendors"))
            {
                using (var cmd = new SqlCommand("dbo.DeleteVendorItem", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VendorId", vendorId).DbType = DbType.Int32;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

    }
}
