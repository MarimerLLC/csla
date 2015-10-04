#if NETFX_CORE || (ANDROID || IOS)
//-----------------------------------------------------------------------
// <copyright file="BindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Base collection class that supports serialization to/from</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Csla.Serialization.Mobile;
using Csla.Core.FieldManager;
using Csla.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Csla.Properties;

namespace Csla.Core
{
    /// <summary>
    /// Base collection class that supports serialization to/from
    /// a .NET BindingList.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BindingList<T> : ObservableCollection<T>, IBindingList
    {
        private bool _supportsChangeNotificationCore = true;
        /// <summary>
        /// Gets a value indicating whether this object
        /// supports change notification.
        /// </summary>
        protected virtual bool SupportsChangeNotificationCore { get { return _supportsChangeNotificationCore; } }
        //protected bool IsReadOnlyCore { get; set; }

        #region IBindingList Members

        private bool _raiseListChangedEvents = true;
        private bool _allowEdit;
        private bool _allowNew;
        private bool _allowRemove;

        /// <summary>
        /// Gets or sets a value indicating whether data binding
        /// can automatically edit items in this collection.
        /// </summary>
        public bool AllowEdit
        {
            get { return _allowEdit; }
            set { _allowEdit = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether data binding
        /// can automatically add new items to this collection.
        /// </summary>
        public bool AllowNew
        {
            get { return _allowNew; }
            set { _allowNew = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether data binding
        /// can automatically remove items from this collection.
        /// </summary>
        public bool AllowRemove
        {
            get { return _allowRemove; }
            set { _allowRemove = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// collection should raise changed events.
        /// </summary>
        public bool RaiseListChangedEvents { get { return _raiseListChangedEvents; } set { _raiseListChangedEvents = value; } }

        /// <summary>
        /// Begins an asyncronous operation to
        /// add a new item to this collection.
        /// </summary>
        public void AddNew()
        {
            AddNewCore();
        }

        void IBindingList.AddNew()
        {
            AddNew();
        }

        #endregion

        /// <summary>
        /// Event raised when a new object has been 
        /// added to the collection.
        /// </summary>
        public event EventHandler<AddedNewEventArgs<T>> AddedNew;

        /// <summary>
        /// Raises the AddedNew event.
        /// </summary>
        /// <param name="item"></param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void OnAddedNew(T item)
        {
            var args = new AddedNewEventArgs<T>(item);
            if (AddedNew != null)
                AddedNew(this, args);
        }

        /// <summary>
        /// Override this method to start an asynchronous
        /// operation to create a new object that is added
        /// to the collection. You MUST call OnCoreAdded()
        /// when the asynchronous operation is complete.
        /// </summary>
        protected virtual void AddNewCore()
        {
            throw new NotImplementedException(Resources.AddNewCoreMustBeOverriden);
        }

        /// <summary>
        /// Handler called when the async AddNewCore
        /// operation is complete. This method adds the
        /// new item to the collection and raises
        /// the AddedNew event.
        /// </summary>
        /// <param name="sender">
        /// Calling object.
        /// </param>
        /// <param name="e">
        /// Object containing the results of the asynchronous operation.
        /// </param>
        protected virtual void OnCoreAdded(object sender, DataPortalResult<T> e)
        {
            Add(e.Object);
            OnAddedNew(e.Object);
        }

        /// <summary>
        /// Raises the CollectionChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SupportsChangeNotificationCore && RaiseListChangedEvents)
                base.OnCollectionChanged(e);
        }
    }
}
#endif