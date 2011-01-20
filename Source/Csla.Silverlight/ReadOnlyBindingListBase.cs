//-----------------------------------------------------------------------
// <copyright file="ReadOnlyBindingListBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>This is the base class from which readonly collections</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization;

namespace Csla
{
  /// <summary>
  /// This is the base class from which readonly collections
  /// of readonly objects should be derived.
  /// </summary>
  /// <typeparam name="T">Type of the list class.</typeparam>
  /// <typeparam name="C">Type of child objects contained in the list.</typeparam>
  [Serializable]
  public abstract class ReadOnlyBindingListBase<T, C> : ReadOnlyListBase<T, C>
    where T : ReadOnlyBindingListBase<T, C>
  { }
}