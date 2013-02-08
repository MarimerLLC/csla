//-----------------------------------------------------------------------
// <copyright file="ObservableCollection.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Android implementation of ObservableCollection</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime;
using System.Threading;

namespace System.Collections.ObjectModel
{

  [Serializable]
  public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
  {
    #region " Attributes "
    private Blocker<T> _monitor;
    [NonSerialized]
    private NotifyCollectionChangedEventHandler _collectionChangedHandler;
    [NonSerialized]
    private PropertyChangedEventHandler _propertyChangedHandler;
    #endregion

    #region " Events "

    public event Specialized.NotifyCollectionChangedEventHandler CollectionChanged
    {
      add
      {
        NotifyCollectionChangedEventHandler handler2;
        NotifyCollectionChangedEventHandler collectionChanged = this._collectionChangedHandler;
        do
        {
          handler2 = collectionChanged;
          NotifyCollectionChangedEventHandler handler3 = (NotifyCollectionChangedEventHandler)Delegate.Combine(handler2, value);
          collectionChanged = Interlocked.CompareExchange<NotifyCollectionChangedEventHandler>(ref this._collectionChangedHandler, handler3, handler2);
        }
        while (collectionChanged != handler2);
      }
      remove
      {
        NotifyCollectionChangedEventHandler handler2;
        NotifyCollectionChangedEventHandler collectionChanged = this._collectionChangedHandler;
        do
        {
          handler2 = collectionChanged;
          NotifyCollectionChangedEventHandler handler3 = (NotifyCollectionChangedEventHandler)Delegate.Remove(handler2, value);
          collectionChanged = Interlocked.CompareExchange<NotifyCollectionChangedEventHandler>(ref this._collectionChangedHandler, handler3, handler2);
        }
        while (collectionChanged != handler2);
      }
    }

    protected event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
        PropertyChangedEventHandler handler2;
        PropertyChangedEventHandler propertyChanged = this._propertyChangedHandler;
        do
        {
          handler2 = propertyChanged;
          PropertyChangedEventHandler handler3 = (PropertyChangedEventHandler)Delegate.Combine(handler2, value);
          propertyChanged = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this._propertyChangedHandler, handler3, handler2);
        }
        while (propertyChanged != handler2);
      }
      remove
      {
        PropertyChangedEventHandler handler2;
        PropertyChangedEventHandler propertyChanged = this._propertyChangedHandler;
        do
        {
          handler2 = propertyChanged;
          PropertyChangedEventHandler handler3 = (PropertyChangedEventHandler)Delegate.Remove(handler2, value);
          propertyChanged = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this._propertyChangedHandler, handler3, handler2);
        }
        while (propertyChanged != handler2);
      }
    }

    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
      add
      {
        this._propertyChangedHandler += value;
      }
      remove
      {
        this._propertyChangedHandler -= value;
      }
    }

    #endregion

    #region " Meothds "

    public ObservableCollection()
    {
      this._monitor = new Blocker<T>();
    }

    public ObservableCollection(IEnumerable<T> collection)
    {
      this._monitor = new Blocker<T>();
      if (collection == null)
      {
        throw new ArgumentNullException("collection");
      }
      this.CopyFrom(collection);
    }

    public ObservableCollection(List<T> list) : base((list != null) ? new List<T>(list.Count) : list)
    {
      this._monitor = new Blocker<T>();
      this.CopyFrom(list);
    }

    protected IDisposable BlockReentrancy()
    {
      this._monitor.Enter();
      return this._monitor;
    }

    protected void IsRentry()
    {
      if ((this._monitor.Busy && (this._collectionChangedHandler != null)) && (this._collectionChangedHandler.GetInvocationList().Length > 1))
      {
        throw new InvalidOperationException("Observable Collection Reentrancy Not Allowed");
      }
    }

    protected override void ClearItems()
    {
      this.IsRentry();
      base.ClearItems();
      this.OnPropertyChanged("Count");
      this.OnPropertyChanged("Item[]");
      this.OnCollectionReset();
    }

    private void CopyFrom(IEnumerable<T> collection)
    {
      IList<T> items = base.Items;
      if ((collection != null) && (items != null))
      {
        using (IEnumerator<T> enumerator = collection.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            items.Add(enumerator.Current);
          }
        }
      }
    }

    protected override void InsertItem(int index, T item)
    {
      this.IsRentry();
      base.InsertItem(index, item);
      this.OnPropertyChanged("Count");
      this.OnPropertyChanged("Item[]");
      this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
    }

    public void Move(int oldIndex, int newIndex)
    {
      this.MoveItem(oldIndex, newIndex);
    }

    protected virtual void MoveItem(int oldIndex, int newIndex)
    {
      this.IsRentry();
      T item = base[oldIndex];
      base.RemoveItem(oldIndex);
      base.InsertItem(newIndex, item);
      this.OnPropertyChanged("Item[]");
      this.OnCollectionChanged(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
    }

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (this._collectionChangedHandler != null)
      {
        using (this.BlockReentrancy())
        {
          this._collectionChangedHandler(this, e);
        }
      }
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
    }

    private void OnCollectionReset()
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (this._propertyChangedHandler != null)
      {
        this._propertyChangedHandler(this, e);
      }
    }

    private void OnPropertyChanged(string propertyName)
    {
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected override void RemoveItem(int index)
    {
      this.IsRentry();
      T item = base[index];
      base.RemoveItem(index);
      this.OnPropertyChanged("Count");
      this.OnPropertyChanged("Item[]");
      this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
    }

    protected override void SetItem(int index, T item)
    {
      this.IsRentry();
      T oldItem = base[index];
      base.SetItem(index, item);
      this.OnPropertyChanged("Item[]");
      this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
    }

    #endregion

    #region " Nested Types "
    [Serializable]
    private class Blocker<S> : IDisposable
    {
      // Fields
      private int _count;

      // Methods
      public void Dispose()
      {
        this._count--;
      }

      public void Enter()
      {
        this._count++;
      }

      // Properties
      public bool Busy
      {
        get
        {
          return (this._count > 0);
        }
      }
    }
    #endregion

  }
}

