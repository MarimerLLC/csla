using System;
using System.Collections.Generic;
using Csla;
using ProjectsVendors.DataAccess;

namespace ProjectsVendors.Business
{

    /// <summary>
    /// VendorCollection (editable child list).<br/>
    /// This is a generated <see cref="VendorCollection"/> business object.
    /// </summary>
    /// <remarks>
    /// This class is child of <see cref="ProjectEdit"/> editable root object.<br/>
    /// The items of the collection are <see cref="VendorItem"/> objects.
    /// </remarks>
    [Serializable]
#if WINFORMS
    public partial class VendorCollection : BusinessBindingListBase<VendorCollection, VendorItem>
#else
    public partial class VendorCollection : BusinessListBase<VendorCollection, VendorItem>
#endif
    {

        #region Collection Business Methods

        /// <summary>
        /// Removes a <see cref="VendorItem"/> item from the collection.
        /// </summary>
        /// <param name="vendorId">The VendorId of the item to be removed.</param>
        public void Remove(int vendorId)
        {
            foreach (var vendorItem in this)
            {
                if (vendorItem.VendorId == vendorId)
                {
                    Remove(vendorItem);
                    break;
                }
            }
        }

        /// <summary>
        /// Determines whether a <see cref="VendorItem"/> item is in the collection.
        /// </summary>
        /// <param name="vendorId">The VendorId of the item to search for.</param>
        /// <returns><c>true</c> if the VendorItem is a collection item; otherwise, <c>false</c>.</returns>
        public bool Contains(int vendorId)
        {
            foreach (var vendorItem in this)
            {
                if (vendorItem.VendorId == vendorId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether a <see cref="VendorItem"/> item is in the collection's DeletedList.
        /// </summary>
        /// <param name="vendorId">The VendorId of the item to search for.</param>
        /// <returns><c>true</c> if the VendorItem is a deleted collection item; otherwise, <c>false</c>.</returns>
        public bool ContainsDeleted(int vendorId)
        {
            foreach (var vendorItem in DeletedList)
            {
                if (vendorItem.VendorId == vendorId)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="VendorCollection"/> class.
        /// </summary>
        /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public VendorCollection()
        {
            // Use factory methods and do not use direct creation.

            // show the framework that this is a child object
            MarkAsChild();

            var rlce = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            AllowNew = true;
            AllowEdit = true;
            AllowRemove = true;
            RaiseListChangedEvents = rlce;
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Loads a <see cref="VendorCollection"/> collection from the database, based on given criteria.
        /// </summary>
        /// <param name="projectId">The Project Id.</param>
        protected void DataPortal_Fetch(int projectId)
        {
            var args = new DataPortalHookArgs(projectId);
            OnFetchPre(args);
            using (var dalManager = DalFactoryProjectsVendors.GetManager())
            {
                var dal = dalManager.GetProvider<IVendorCollectionDal>();
                var data = dal.Fetch(projectId);
                Fetch(data);
            }
            OnFetchPost(args);
        }

        /// <summary>
        /// Loads all <see cref="VendorCollection"/> collection items from the given list of VendorItemDto.
        /// </summary>
        /// <param name="data">The list of <see cref="VendorItemDto"/>.</param>
        private void Fetch(List<VendorItemDto> data)
        {
            var rlce = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            foreach (var dto in data)
            {
                Add(DataPortal.FetchChild<VendorItem>(dto));
            }
            RaiseListChangedEvents = rlce;
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
