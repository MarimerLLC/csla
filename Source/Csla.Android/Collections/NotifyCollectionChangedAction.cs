//-----------------------------------------------------------------------
// <copyright file="NotifyCollectionChangedAction.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Android implementation of NotifyCollectionChangedAction</summary>
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
  public enum NotifyCollectionChangedAction
  {
    Add,
    Remove,
    Replace,
    Reset
  }
}