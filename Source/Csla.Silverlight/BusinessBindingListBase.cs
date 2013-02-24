//-----------------------------------------------------------------------
// <copyright file="BusinessBindingListBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>This is the base class from which most business collections</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization;

namespace Csla
{
  /// <summary>
  /// This is the base class from which most business collections
  /// or lists will be derived.
  /// </summary>
  /// <typeparam name="T">Type of the business object being defined.</typeparam>
  /// <typeparam name="C">Type of the child objects contained in the list.</typeparam>
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [Serializable]
  public abstract class BusinessBindingListBase<T, C> : BusinessListBase<T, C>
    where T : BusinessBindingListBase<T, C>
    where C : Csla.Core.IEditableBusinessObject
  {

  }
}