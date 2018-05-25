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
    /// DAL SQL Server implementation of <see cref="IProjectListDal"/>
    /// </summary>
    public partial class ProjectListDal : IProjectListDal
    {

        #region DAL methods

        /// <summary>
        /// Loads a ProjectList collection from the database.
        /// </summary>
        /// <returns>A list of <see cref="ProjectInfoDto"/>.</returns>
        public List<ProjectInfoDto> Fetch()
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("ProjectsVendors"))
            {
                using (var cmd = new SqlCommand("dbo.GetProjectList", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var dr = cmd.ExecuteReader();
                    return LoadCollection(dr);
                }
            }
        }

        private List<ProjectInfoDto> LoadCollection(IDataReader data)
        {
            var projectList = new List<ProjectInfoDto>();
            using (var dr = new SafeDataReader(data))
            {
                while (dr.Read())
                {
                    projectList.Add(Fetch(dr));
                }
            }
            return projectList;
        }

        private ProjectInfoDto Fetch(SafeDataReader dr)
        {
            var projectInfo = new ProjectInfoDto();
            // Value properties
            projectInfo.ProjectId = dr.GetInt32("ProjectId");
            projectInfo.ProjectName = dr.GetString("ProjectName");

            return projectInfo;
        }

        #endregion

    }
}
