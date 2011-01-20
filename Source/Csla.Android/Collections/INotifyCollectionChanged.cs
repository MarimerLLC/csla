//-----------------------------------------------------------------------
// <copyright file="INotifyCollectionChanged.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Android implementation of INotifyCollectionChanged</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace System.Collections.Specialized
{
  public interface INotifyCollectionChanged
  {
    event NotifyCollectionChangedEventHandler CollectionChanged;
  }
}