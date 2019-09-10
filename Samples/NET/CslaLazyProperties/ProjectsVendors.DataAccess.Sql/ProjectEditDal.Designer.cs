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
    /// DAL SQL Server implementation of <see cref="IProjectEditDal"/>
    /// </summary>
    public partial class ProjectEditDal : IProjectEditDal
    {

        #region DAL methods

        /// <summary>
        /// Loads a ProjectEdit object from the database.
        /// </summary>
        /// <param name="projectId">The fetch criteria.</param>
        /// <returns>A ProjectEditDto object.</returns>
        public ProjectEditDto Fetch(int projectId)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("ProjectsVendors"))
            {
                using (var cmd = new SqlCommand("dbo.GetProjectEdit", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectId", projectId).DbType = DbType.Int32;
                    var dr = cmd.ExecuteReader();
                    return Fetch(dr);
                }
            }
        }

        private ProjectEditDto Fetch(IDataReader data)
        {
            var projectEdit = new ProjectEditDto();
            using (var dr = new SafeDataReader(data))
            {
                if (dr.Read())
                {
                    projectEdit.ProjectId = dr.GetInt32("ProjectId");
                    projectEdit.ProjectName = dr.GetString("ProjectName");
                    projectEdit.StartDate = dr.GetSmartDate("StartDate", true);
                    projectEdit.DeliveryDate = dr.GetSmartDate("DeliveryDate", true);
                }
            }
            return projectEdit;
        }

        /// <summary>
        /// Inserts a new ProjectEdit object in the database.
        /// </summary>
        /// <param name="projectEdit">The Project Edit DTO.</param>
        /// <returns>The new <see cref="ProjectEditDto"/>.</returns>
        public ProjectEditDto Insert(ProjectEditDto projectEdit)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("ProjectsVendors"))
            {
                using (var cmd = new SqlCommand("dbo.AddProjectEdit", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectId", projectEdit.ProjectId).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@ProjectName", projectEdit.ProjectName).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@StartDate", projectEdit.StartDate.DBValue).DbType = DbType.DateTime2;
                    cmd.Parameters.AddWithValue("@DeliveryDate", projectEdit.DeliveryDate.DBValue).DbType = DbType.DateTime2;
                    cmd.ExecuteNonQuery();
                    projectEdit.ProjectId = (int)cmd.Parameters["@ProjectId"].Value;
                }
            }
            return projectEdit;
        }

        /// <summary>
        /// Updates in the database all changes made to the ProjectEdit object.
        /// </summary>
        /// <param name="projectEdit">The Project Edit DTO.</param>
        /// <returns>The updated <see cref="ProjectEditDto"/>.</returns>
        public ProjectEditDto Update(ProjectEditDto projectEdit)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("ProjectsVendors"))
            {
                using (var cmd = new SqlCommand("dbo.UpdateProjectEdit", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectId", projectEdit.ProjectId).DbType = DbType.Int32;
                    cmd.Parameters.AddWithValue("@ProjectName", projectEdit.ProjectName).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@StartDate", projectEdit.StartDate.DBValue).DbType = DbType.DateTime2;
                    cmd.Parameters.AddWithValue("@DeliveryDate", projectEdit.DeliveryDate.DBValue).DbType = DbType.DateTime2;
                    var rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        throw new DataNotFoundException("ProjectEdit");
                }
            }
            return projectEdit;
        }

        /// <summary>
        /// Deletes the ProjectEdit object from database.
        /// </summary>
        /// <param name="projectId">The delete criteria.</param>
        public void Delete(int projectId)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("ProjectsVendors"))
            {
                using (var cmd = new SqlCommand("dbo.DeleteProjectEdit", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectId", projectId).DbType = DbType.Int32;
                    var rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        throw new DataNotFoundException("ProjectEdit");
                }
            }
        }

        #endregion

    }
}
