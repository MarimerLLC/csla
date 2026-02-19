//-----------------------------------------------------------------------
// <copyright file="IBusinessDocumentBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Consolidated interface for the BusinessDocumentBase type.</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla
{
  /// <summary>
  /// Consolidated interface for the BusinessDocumentBase type,
  /// which combines BusinessBase and BusinessListBase capabilities.
  /// A business document has its own properties AND contains
  /// a collection of child items.
  /// </summary>
  /// <typeparam name="C">Type of the child objects contained in the collection.</typeparam>
  public interface IBusinessDocumentBase<C> :
    IBusinessBase,
    IBusinessListBase<C>
    where C : IEditableBusinessObject
  {
  }
}
