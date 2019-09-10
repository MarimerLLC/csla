using System;
using System.Collections.Generic;
using Csla;
using ProjectsVendors.DataAccess;

namespace ProjectsVendors.Business
{

    /// <summary>
    /// ProjectList (read only list).<br/>
    /// This is a generated <see cref="ProjectList"/> business object.
    /// This class is a root collection.
    /// </summary>
    /// <remarks>
    /// The items of the collection are <see cref="ProjectInfo"/> objects.
    /// </remarks>
    [Serializable]
#if WINFORMS
    public partial class ProjectList : ReadOnlyBindingListBase<ProjectList, ProjectInfo>
#else
    public partial class ProjectList : ReadOnlyListBase<ProjectList, ProjectInfo>
#endif
    {

        #region Collection Business Methods

        /// <summary>
        /// Determines whether a <see cref="ProjectInfo"/> item is in the collection.
        /// </summary>
        /// <param name="projectId">The ProjectId of the item to search for.</param>
        /// <returns><c>true</c> if the ProjectInfo is a collection item; otherwise, <c>false</c>.</returns>
        public bool Contains(int projectId)
        {
            foreach (var projectInfo in this)
            {
                if (projectInfo.ProjectId == projectId)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Factory method. Loads a <see cref="ProjectList"/> collection.
        /// </summary>
        /// <returns>A reference to the fetched <see cref="ProjectList"/> collection.</returns>
        public static ProjectList GetProjectList()
        {
            return DataPortal.Fetch<ProjectList>();
        }

        /// <summary>
        /// Factory method. Asynchronously loads a <see cref="ProjectList"/> collection.
        /// </summary>
        /// <param name="callback">The completion callback method.</param>
        public static void GetProjectList(EventHandler<DataPortalResult<ProjectList>> callback)
        {
            DataPortal.BeginFetch<ProjectList>(callback);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectList"/> class.
        /// </summary>
        /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ProjectList()
        {
            // Use factory methods and do not use direct creation.

            var rlce = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            AllowNew = false;
            AllowEdit = false;
            AllowRemove = false;
            RaiseListChangedEvents = rlce;
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Loads a <see cref="ProjectList"/> collection from the database.
        /// </summary>
        protected void DataPortal_Fetch()
        {
            var args = new DataPortalHookArgs();
            OnFetchPre(args);
            using (var dalManager = DalFactoryProjectsVendors.GetManager())
            {
                var dal = dalManager.GetProvider<IProjectListDal>();
                var data = dal.Fetch();
                Fetch(data);
            }
            OnFetchPost(args);
        }

        /// <summary>
        /// Loads all <see cref="ProjectList"/> collection items from the given list of ProjectInfoDto.
        /// </summary>
        /// <param name="data">The list of <see cref="ProjectInfoDto"/>.</param>
        private void Fetch(List<ProjectInfoDto> data)
        {
            IsReadOnly = false;
            var rlce = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            foreach (var dto in data)
            {
                Add(DataPortal.FetchChild<ProjectInfo>(dto));
            }
            RaiseListChangedEvents = rlce;
            IsReadOnly = true;
        }

        #endregion

        #region DataPortal Hooks

        /// <summary>
        /// Occurs after setting query parameters and before the fetch operation.
        /// </summary>
        partial void OnFetchPre(DataPortalHookArgs args);

        /// <summary>
        /// Occurs after the fetch operation (object or collection is fully loaded and set up).
        /// </summary>
        partial void OnFetchPost(DataPortalHookArgs args);

        #endregion

    }
}
