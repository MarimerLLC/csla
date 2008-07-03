using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core.FieldManager;
using Csla.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Csla.Core
{
  public class BindingList<T> : ObservableCollection<T>
  {
    private bool _supportsChangeNotificationCore = true;
    protected virtual bool SupportsChangeNotificationCore { get { return _supportsChangeNotificationCore; } }
    //protected bool IsReadOnlyCore { get; set; }

    #region IBindingList Members
    
    public bool AllowEdit { get; set; }
    public bool AllowNew { get; set; }
    public bool AllowRemove { get; set; }
    public bool RaiseListChangedEvents { get; set; }

    public void AddNew()
    {
      AddNewCore();
    }

    #endregion

    public event EventHandler<AddedNewEventArgs<T>> AddedNew;

    public virtual void OnAddedNew(T item)
    {
      if (AddedNew != null)
        AddedNew(this, new AddedNewEventArgs<T>(item));
    }

    protected virtual void AddNewCore()
    {
      throw new NotImplementedException("Add new core must be overriden");
    }

    protected virtual void OnCoreAdded(object sender, DataPortalResult<T> e)
    {
      Add(e.Object);
      OnAddedNew(e.Object);
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if(SupportsChangeNotificationCore)
        base.OnCollectionChanged(e);
    }
  }
}
