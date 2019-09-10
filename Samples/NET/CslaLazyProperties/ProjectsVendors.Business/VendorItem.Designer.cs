using System;
using Csla;
using ProjectsVendors.DataAccess;

namespace ProjectsVendors.Business
{

    /// <summary>
    /// VendorItem (editable child object).<br/>
    /// This is a generated <see cref="VendorItem"/> business object.
    /// </summary>
    /// <remarks>
    /// This class is an item of <see cref="VendorCollection"/> collection.
    /// </remarks>
    [Serializable]
    public partial class VendorItem : BusinessBase<VendorItem>
    {

        #region Static Fields

        private static int _lastId;

        #endregion

        #region Business Properties

        /// <summary>
        /// Maintains metadata about <see cref="VendorId"/> property.
        /// </summary>
        [NotUndoable]
        public static readonly PropertyInfo<int> VendorIdProperty = RegisterProperty<int>(p => p.VendorId, "Vendor Id");
        /// <summary>
        /// Gets the Vendor Id.
        /// </summary>
        /// <value>The Vendor Id.</value>
        public int VendorId
        {
            get { return GetProperty(VendorIdProperty); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="VendorName"/> property.
        /// </summary>
        public static readonly PropertyInfo<string> VendorNameProperty = RegisterProperty<string>(p => p.VendorName, "Vendor Name");
        /// <summary>
        /// Gets or sets the Vendor Name.
        /// </summary>
        /// <value>The Vendor Name.</value>
        public string VendorName
        {
            get { return GetProperty(VendorNameProperty); }
            set { SetProperty(VendorNameProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="VendorContact"/> property.
        /// </summary>
        public static readonly PropertyInfo<string> VendorContactProperty = RegisterProperty<string>(p => p.VendorContact, "Vendor Contact");
        /// <summary>
        /// Gets or sets the Vendor Contact.
        /// </summary>
        /// <value>The Vendor Contact.</value>
        public string VendorContact
        {
            get { return GetProperty(VendorContactProperty); }
            set { SetProperty(VendorContactProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="VendorPhone"/> property.
        /// </summary>
        public static readonly PropertyInfo<string> VendorPhoneProperty = RegisterProperty<string>(p => p.VendorPhone, "Vendor Phone");
        /// <summary>
        /// Gets or sets the Vendor Phone.
        /// </summary>
        /// <value>The Vendor Phone.</value>
        public string VendorPhone
        {
            get { return GetProperty(VendorPhoneProperty); }
            set { SetProperty(VendorPhoneProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="VendorEmail"/> property.
        /// </summary>
        public static readonly PropertyInfo<string> VendorEmailProperty = RegisterProperty<string>(p => p.VendorEmail, "Vendor Email");
        /// <summary>
        /// Gets or sets the Vendor Email.
        /// </summary>
        /// <value>The Vendor Email.</value>
        public string VendorEmail
        {
            get { return GetProperty(VendorEmailProperty); }
            set { SetProperty(VendorEmailProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="IsPrimaryVendor"/> property.
        /// </summary>
        public static readonly PropertyInfo<bool> IsPrimaryVendorProperty = RegisterProperty<bool>(p => p.IsPrimaryVendor, "Is Primary Vendor");
        /// <summary>
        /// Gets or sets the Is Primary Vendor.
        /// </summary>
        /// <value><c>true</c> if Is Primary Vendor; otherwise, <c>false</c>.</value>
        public bool IsPrimaryVendor
        {
            get { return GetProperty(IsPrimaryVendorProperty); }
            set { SetProperty(IsPrimaryVendorProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="LastUpdated"/> property.
        /// </summary>
        [NotUndoable]
        public static readonly PropertyInfo<SmartDate> LastUpdatedProperty = RegisterProperty<SmartDate>(p => p.LastUpdated, "Last Updated");
        /// <summary>
        /// Gets the Last Updated.
        /// </summary>
        /// <value>The Last Updated.</value>
        public SmartDate LastUpdated
        {
            get { return GetProperty(LastUpdatedProperty); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="VendorItem"/> class.
        /// </summary>
        /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public VendorItem()
        {
            // Use factory methods and do not use direct creation.

            // show the framework that this is a child object
            MarkAsChild();
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Loads default values for the <see cref="VendorItem"/> object properties.
        /// </summary>
        [RunLocal]
        protected override void Child_Create()
        {
            LoadProperty(VendorIdProperty, System.Threading.Interlocked.Decrement(ref _lastId));
            LoadProperty(LastUpdatedProperty, new SmartDate(DateTime.Now));
            var args = new DataPortalHookArgs();
            OnCreate(args);
            base.Child_Create();
        }

        /// <summary>
        /// Loads a <see cref="VendorItem"/> object from the given <see cref="VendorItemDto"/>.
        /// </summary>
        /// <param name="data">The VendorItemDto to use.</param>
        private void Child_Fetch(VendorItemDto data)
        {
            // Value properties
            LoadProperty(VendorIdProperty, data.VendorId);
            LoadProperty(VendorNameProperty, data.VendorName);
            LoadProperty(VendorContactProperty, data.VendorContact);
            LoadProperty(VendorPhoneProperty, data.VendorPhone);
            LoadProperty(VendorEmailProperty, data.VendorEmail);
            LoadProperty(IsPrimaryVendorProperty, data.IsPrimaryVendor);
            LoadProperty(LastUpdatedProperty, data.LastUpdated);
            var args = new DataPortalHookArgs(data);
            OnFetchRead(args);
            // check all object rules and property rules
            BusinessRules.CheckRules();
        }

        /// <summary>
        /// Inserts a new <see cref="VendorItem"/> object in the database.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        [Transactional(TransactionalTypes.TransactionScope)]
        private void Child_Insert(ProjectEdit parent)
        {
            SimpleAuditTrail();
            var dto = new VendorItemDto();
            dto.Parent_ProjectId = parent.ProjectId;
            dto.VendorName = VendorName;
            dto.VendorContact = VendorContact;
            dto.VendorPhone = VendorPhone;
            dto.VendorEmail = VendorEmail;
            dto.IsPrimaryVendor = IsPrimaryVendor;
            dto.LastUpdated = LastUpdated;
            using (var dalManager = DalFactoryProjectsVendors.GetManager())
            {
                var args = new DataPortalHookArgs(dto);
                OnInsertPre(args);
                var dal = dalManager.GetProvider<IVendorItemDal>();
                using (BypassPropertyChecks)
                {
                    var resultDto = dal.Insert(dto);
                    LoadProperty(VendorIdProperty, resultDto.VendorId);
                    args = new DataPortalHookArgs(resultDto);
                }
                OnInsertPost(args);
            }
        }

        /// <summary>
        /// Updates in the database all changes made to the <see cref="VendorItem"/> object.
        /// </summary>
        [Transactional(TransactionalTypes.TransactionScope)]
        private void Child_Update()
        {
            if (!IsDirty)
                return;

            SimpleAuditTrail();
            var dto = new VendorItemDto();
            dto.VendorId = VendorId;
            dto.VendorName = VendorName;
            dto.VendorContact = VendorContact;
            dto.VendorPhone = VendorPhone;
            dto.VendorEmail = VendorEmail;
            dto.IsPrimaryVendor = IsPrimaryVendor;
            dto.LastUpdated = LastUpdated;
            using (var dalManager = DalFactoryProjectsVendors.GetManager())
            {
                var args = new DataPortalHookArgs(dto);
                OnUpdatePre(args);
                var dal = dalManager.GetProvider<IVendorItemDal>();
                using (BypassPropertyChecks)
                {
                    var resultDto = dal.Update(dto);
                    args = new DataPortalHookArgs(resultDto);
                }
                OnUpdatePost(args);
            }
        }

        private void SimpleAuditTrail()
        {
            LoadProperty(LastUpdatedProperty, DateTime.Now);
        }

        /// <summary>
        /// Self deletes the <see cref="VendorItem"/> object from database.
        /// </summary>
        [Transactional(TransactionalTypes.TransactionScope)]
        private void Child_DeleteSelf()
        {
            // audit the object, just in case soft delete is used on this object
            SimpleAuditTrail();
            using (var dalManager = DalFactoryProjectsVendors.GetManager())
            {
                var args = new DataPortalHookArgs();
                OnDeletePre(args);
                var dal = dalManager.GetProvider<IVendorItemDal>();
                using (BypassPropertyChecks)
                {
                    dal.Delete(ReadProperty(VendorIdProperty));
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
