using System;
using Csla;
using ProjectsVendors.DataAccess;

namespace ProjectsVendors.Business
{

    /// <summary>
    /// ProjectEdit (editable root object).<br/>
    /// This is a generated <see cref="ProjectEdit"/> business object.
    /// </summary>
    /// <remarks>
    /// This class contains one child collection:<br/>
    /// - <see cref="Vendors"/> of type <see cref="VendorCollection"/> (1:M relation to <see cref="VendorItem"/>)
    /// </remarks>
    [Serializable]
    public partial class ProjectEdit : BusinessBase<ProjectEdit>
    {

        #region Static Fields

        private static int _lastId;

        #endregion

        #region Business Properties

        /// <summary>
        /// Maintains metadata about <see cref="ProjectId"/> property.
        /// </summary>
        [NotUndoable]
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
        /// Gets or sets the Project Name.
        /// </summary>
        /// <value>The Project Name.</value>
        public string ProjectName
        {
            get { return GetProperty(ProjectNameProperty); }
            set { SetProperty(ProjectNameProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="StartDate"/> property.
        /// </summary>
        public static readonly PropertyInfo<SmartDate> StartDateProperty = RegisterProperty<SmartDate>(p => p.StartDate, "Start Date");
        /// <summary>
        /// Gets or sets the Start Date.
        /// </summary>
        /// <value>The Start Date.</value>
        public string StartDate
        {
            get { return GetPropertyConvert<SmartDate, string>(StartDateProperty); }
            set { SetPropertyConvert<SmartDate, string>(StartDateProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="DeliveryDate"/> property.
        /// </summary>
        public static readonly PropertyInfo<SmartDate> DeliveryDateProperty = RegisterProperty<SmartDate>(p => p.DeliveryDate, "Delivery Date");
        /// <summary>
        /// Gets or sets the Delivery Date.
        /// </summary>
        /// <value>The Delivery Date.</value>
        public string DeliveryDate
        {
            get { return GetPropertyConvert<SmartDate, string>(DeliveryDateProperty); }
            set { SetPropertyConvert<SmartDate, string>(DeliveryDateProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="IsLazyloaded"/> property.
        /// </summary>
        public static readonly PropertyInfo<string> IsLazyloadedProperty = RegisterProperty<string>(p => p.IsLazyloaded, "Is Lazyloaded");
        /// <summary>
        /// Gets or sets the Is Lazyloaded.
        /// </summary>
        /// <value>The Is Lazyloaded.</value>
        public string IsLazyloaded
        {
            get { return GetProperty(IsLazyloadedProperty); }
            set { SetProperty(IsLazyloadedProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about child <see cref="Vendors"/> property.
        /// </summary>
        public static readonly PropertyInfo<VendorCollection> VendorsProperty = RegisterProperty<VendorCollection>(p => p.Vendors, "Vendors", RelationshipTypes.Child | RelationshipTypes.LazyLoad);
        /// <summary>
        /// Gets the Vendors ("lazy load" child property).
        /// </summary>
        /// <value>The Vendors.</value>
        public VendorCollection Vendors
        {
            get
            {
#if ASYNC
                return LazyGetPropertyAsync(VendorsProperty, DataPortal.FetchAsync<VendorCollection>(ReadProperty(ProjectIdProperty)));
#else
                return LazyGetProperty(VendorsProperty, () => DataPortal.Fetch<VendorCollection>(ReadProperty(ProjectIdProperty)));
#endif
            }
            private set { LoadProperty(VendorsProperty, value); }
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Factory method. Creates a new <see cref="ProjectEdit"/> object.
        /// </summary>
        /// <returns>A reference to the created <see cref="ProjectEdit"/> object.</returns>
        public static ProjectEdit NewProjectEdit()
        {
            return DataPortal.Create<ProjectEdit>();
        }

        /// <summary>
        /// Factory method. Loads a <see cref="ProjectEdit"/> object, based on given parameters.
        /// </summary>
        /// <param name="projectId">The ProjectId parameter of the ProjectEdit to fetch.</param>
        /// <returns>A reference to the fetched <see cref="ProjectEdit"/> object.</returns>
        public static ProjectEdit GetProjectEdit(int projectId)
        {
            return DataPortal.Fetch<ProjectEdit>(projectId);
        }

        /// <summary>
        /// Factory method. Deletes a <see cref="ProjectEdit"/> object, based on given parameters.
        /// </summary>
        /// <param name="projectId">The ProjectId of the ProjectEdit to delete.</param>
        public static void DeleteProjectEdit(int projectId)
        {
            DataPortal.Delete<ProjectEdit>(projectId);
        }

        /// <summary>
        /// Factory method. Asynchronously creates a new <see cref="ProjectEdit"/> object.
        /// </summary>
        /// <param name="callback">The completion callback method.</param>
        public static void NewProjectEdit(EventHandler<DataPortalResult<ProjectEdit>> callback)
        {
            DataPortal.BeginCreate<ProjectEdit>(callback);
        }

        /// <summary>
        /// Factory method. Asynchronously loads a <see cref="ProjectEdit"/> object, based on given parameters.
        /// </summary>
        /// <param name="projectId">The ProjectId parameter of the ProjectEdit to fetch.</param>
        /// <param name="callback">The completion callback method.</param>
        public static void GetProjectEdit(int projectId, EventHandler<DataPortalResult<ProjectEdit>> callback)
        {
            DataPortal.BeginFetch<ProjectEdit>(projectId, callback);
        }

        /// <summary>
        /// Factory method. Asynchronously deletes a <see cref="ProjectEdit"/> object, based on given parameters.
        /// </summary>
        /// <param name="projectId">The ProjectId of the ProjectEdit to delete.</param>
        /// <param name="callback">The completion callback method.</param>
        public static void DeleteProjectEdit(int projectId, EventHandler<DataPortalResult<ProjectEdit>> callback)
        {
            DataPortal.BeginDelete<ProjectEdit>(projectId, callback);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEdit"/> class.
        /// </summary>
        /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ProjectEdit()
        {
            // Use factory methods and do not use direct creation.
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Loads default values for the <see cref="ProjectEdit"/> object properties.
        /// </summary>
        [RunLocal]
        protected override void DataPortal_Create()
        {
            LoadProperty(ProjectIdProperty, System.Threading.Interlocked.Decrement(ref _lastId));
            var args = new DataPortalHookArgs();
            OnCreate(args);
            base.DataPortal_Create();
        }

        /// <summary>
        /// Loads a <see cref="ProjectEdit"/> object from the database, based on given criteria.
        /// </summary>
        /// <param name="projectId">The Project Id.</param>
        protected void DataPortal_Fetch(int projectId)
        {
            var args = new DataPortalHookArgs(projectId);
            OnFetchPre(args);
            using (var dalManager = DalFactoryProjectsVendors.GetManager())
            {
                var dal = dalManager.GetProvider<IProjectEditDal>();
                var data = dal.Fetch(projectId);
                Fetch(data);
            }
            OnFetchPost(args);
            // check all object rules and property rules
            BusinessRules.CheckRules();
        }

        /// <summary>
        /// Loads a <see cref="ProjectEdit"/> object from the given <see cref="ProjectEditDto"/>.
        /// </summary>
        /// <param name="data">The ProjectEditDto to use.</param>
        private void Fetch(ProjectEditDto data)
        {
            // Value properties
            LoadProperty(ProjectIdProperty, data.ProjectId);
            LoadProperty(ProjectNameProperty, data.ProjectName);
            LoadProperty(StartDateProperty, data.StartDate);
            LoadProperty(DeliveryDateProperty, data.DeliveryDate);
            var args = new DataPortalHookArgs(data);
            OnFetchRead(args);
        }

        /// <summary>
        /// Inserts a new <see cref="ProjectEdit"/> object in the database.
        /// </summary>
        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Insert()
        {
            var dto = new ProjectEditDto();
            dto.ProjectName = ProjectName;
            dto.StartDate = ReadProperty(StartDateProperty);
            dto.DeliveryDate = ReadProperty(DeliveryDateProperty);
            using (var dalManager = DalFactoryProjectsVendors.GetManager())
            {
                var args = new DataPortalHookArgs(dto);
                OnInsertPre(args);
                var dal = dalManager.GetProvider<IProjectEditDal>();
                using (BypassPropertyChecks)
                {
                    var resultDto = dal.Insert(dto);
                    LoadProperty(ProjectIdProperty, resultDto.ProjectId);
                    args = new DataPortalHookArgs(resultDto);
                }
                OnInsertPost(args);
                // flushes all pending data operations
                FieldManager.UpdateChildren(this);
            }
        }

        /// <summary>
        /// Updates in the database all changes made to the <see cref="ProjectEdit"/> object.
        /// </summary>
        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Update()
        {
            var dto = new ProjectEditDto();
            dto.ProjectId = ProjectId;
            dto.ProjectName = ProjectName;
            dto.StartDate = ReadProperty(StartDateProperty);
            dto.DeliveryDate = ReadProperty(DeliveryDateProperty);
            using (var dalManager = DalFactoryProjectsVendors.GetManager())
            {
                var args = new DataPortalHookArgs(dto);
                OnUpdatePre(args);
                var dal = dalManager.GetProvider<IProjectEditDal>();
                using (BypassPropertyChecks)
                {
                    var resultDto = dal.Update(dto);
                    args = new DataPortalHookArgs(resultDto);
                }
                OnUpdatePost(args);
                // flushes all pending data operations
                FieldManager.UpdateChildren(this);
            }
        }

        /// <summary>
        /// Self deletes the <see cref="ProjectEdit"/> object.
        /// </summary>
        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(ProjectId);
        }

        /// <summary>
        /// Deletes the <see cref="ProjectEdit"/> object from database.
        /// </summary>
        /// <param name="projectId">The delete criteria.</param>
        [Transactional(TransactionalTypes.TransactionScope)]
        private void DataPortal_Delete(int projectId)
        {
            using (var dalManager = DalFactoryProjectsVendors.GetManager())
            {
                var args = new DataPortalHookArgs();
                // flushes all pending data operations
                FieldManager.UpdateChildren(this);
                OnDeletePre(args);
                var dal = dalManager.GetProvider<IProjectEditDal>();
                using (BypassPropertyChecks)
                {
                    dal.Delete(projectId);
                }
                OnDeletePost(args);
            }
        }

        #endregion

        #region DataPortal Hooks

        /// <summary>
        /// Occurs after setting all defaults for object creation.
        /// </summary>
        partial void OnCreate(DataPortalHookArgs args);

        /// <summary>
        /// Occurs in DataPortal_Delete, after setting query parameters and before the delete operation.
        /// </summary>
        partial void OnDeletePre(DataPortalHookArgs args);

        /// <summary>
        /// Occurs in DataPortal_Delete, after the delete operation, before Commit().
        /// </summary>
        partial void OnDeletePost(DataPortalHookArgs args);

        /// <summary>
        /// Occurs after setting query parameters and before the fetch operation.
        /// </summary>
        partial void OnFetchPre(DataPortalHookArgs args);

        /// <summary>
        /// Occurs after the fetch operation (object or collection is fully loaded and set up).
        /// </summary>
        partial void OnFetchPost(DataPortalHookArgs args);

        /// <summary>
        /// Occurs after the low level fetch operation, before the data reader is destroyed.
        /// </summary>
        partial void OnFetchRead(DataPortalHookArgs args);

        /// <summary>
        /// Occurs after setting query parameters and before the update operation.
        /// </summary>
        partial void OnUpdatePre(DataPortalHookArgs args);

        /// <summary>
        /// Occurs in DataPortal_Insert, after the update operation, before setting back row identifiers (RowVersion) and Commit().
        /// </summary>
        partial void OnUpdatePost(DataPortalHookArgs args);

        /// <summary>
        /// Occurs in DataPortal_Insert, after setting query parameters and before the insert operation.
        /// </summary>
        partial void OnInsertPre(DataPortalHookArgs args);

        /// <summary>
        /// Occurs in DataPortal_Insert, after the insert operation, before setting back row identifiers (ID and RowVersion) and Commit().
        /// </summary>
        partial void OnInsertPost(DataPortalHookArgs args);

        #endregion

    }
}
