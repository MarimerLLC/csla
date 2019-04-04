//-----------------------------------------------------------------------
// <copyright file="ISupportUndo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Define the common methods used by the UI to </summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;

namespace Csla.Core
{
  /// <summary>
  /// Define the common methods used by the UI to 
  /// interact with n-level undo.
  /// </summary>
  /// <remarks>
  /// This interface is designed to help UI framework
  /// developers interact with editable business objects.
  /// The CSLA .NET editable base classes already
  /// implement this interface and the required n-level
  /// undo behaviors.
  /// </remarks>
  public interface ISupportUndo
  {
    /// <summary>
    /// Starts a nested edit on the object.
    /// </summary>
    void BeginEdit();
    /// <summary>
    /// Cancels the current edit process, restoring the object's state to
    /// its previous values.
    /// </summary>
    void CancelEdit();
    /// <summary>
    /// Commits the current edit process.
    /// </summary>
    void ApplyEdit();
  }
}