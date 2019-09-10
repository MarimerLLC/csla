using System;
using Csla;
using ProjectsVendors.DataAccess;

namespace ProjectsVendors.Business
{

    /// <summary>
    /// ProjectInfo (read only object).<br/>
    /// This is a generated <see cref="ProjectInfo"/> business object.
    /// </summary>
    /// <remarks>
    /// This class is an item of <see cref="ProjectList"/> collection.
    /// </remarks>
    [Serializable]
    public partial class ProjectInfo : ReadOnlyBase<ProjectInfo>
    {

        #region Business Properties

        /// <summary>
        /// Maintains metadata about <see cref="ProjectId"/> property.
        /// </summary>
        public static readonly PropertyInfo<int> ProjectIdProperty = RegisterProperty<int>(p => p.ProjectId, "Project Id");
        /// <summary>
        /// Gets the Project Id.
        /// </summary>
        /// <value>The Project Id.</value>
        public int ProjectId
        {
            get { return GetProperty(ProjectIdProperty); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="ProjectName"/> property.
        /// </summary>
        public static readonly PropertyInfo<string> ProjectNameProperty = RegisterProperty<string>(p => p.ProjectName, "Project Name");
        /// <summary>
        /// Gets the Project Name.
        /// </summary>
        /// <value>The Project Name.</value>
        public string ProjectName
        {
            get { return GetProperty(ProjectNameProperty); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectInfo"/> class.
        /// </summary>
        /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ProjectInfo()
        {
            // Use factory methods and do not use direct creation.
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Loads a <see cref="ProjectInfo"/> object from the given <see cref="ProjectInfoDto"/>.
        /// </summary>
        /// <param name="data">The ProjectInfoDto to use.</param>
        private void Child_Fetch(ProjectInfoDto data)
        {
            // Value properties
            LoadProperty(ProjectIdProperty, data.ProjectId);
            LoadProperty(ProjectNameProperty, data.ProjectName);
            var args = new DataPortalHookArgs(data);
            OnFetchRead(args);
        }

        #endregion

        #region DataPortal Hooks

        /// <summary>
        /// Occurs after the low level fetch operation, before the data reader is destroyed.
        /// </summary>
        partial void OnFetchRead(DataPortalHookArgs args);

        #endregion

    }
}
