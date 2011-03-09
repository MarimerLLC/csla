//-----------------------------------------------------------------------
// <copyright file="NotifyCollectionChangedEventHandler.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Android implementation of NotifyCollectionChangedEventHandler</summary>
//-----------------------------------------------------------------------

namespace System.Collections.Specialized
{
  // Summary:
  //     Represents the method that handles the System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged
  //     event.
  //
  // Parameters:
  //   sender:
  //     The object that raised the event.
  //
  //   e:
  //     Information about the event.
  public delegate void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e);
}