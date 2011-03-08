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
using Csla.Serialization;

namespace System.Collections.ObjectModel
{
  [Serializable]
  public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
  {
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (CollectionChanged != null)
        CollectionChanged(this, e);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}