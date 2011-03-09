//-----------------------------------------------------------------------
// <copyright file="INotifyCollectionChanged.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Android implementation of INotifyCollectionChanged</summary>
//-----------------------------------------------------------------------

namespace System.Collections.Specialized
{
  public interface INotifyCollectionChanged
  {
    event NotifyCollectionChangedEventHandler CollectionChanged;
  }
}