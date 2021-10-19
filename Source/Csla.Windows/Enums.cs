//-----------------------------------------------------------------------
// <copyright file="Enums.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The possible form actions.</summary>
//-----------------------------------------------------------------------
namespace Csla.Windows
{
  /// <summary>
  /// The possible form actions.
  /// </summary>
  public enum CslaFormAction
  {
    /// <summary>
    /// No action.
    /// </summary>
    None,
    /// <summary>
    /// Perform a save.
    /// </summary>
    Save,
    /// <summary>
    /// Undo changes.
    /// </summary>
    Cancel,
    /// <summary>
    /// Close the form.
    /// </summary>
    Close,
    /// <summary>
    /// Display a message box with any broken rules
    /// </summary>
    Validate
  }

  /// <summary>
  /// The possible actions for post save.
  /// </summary>
  public enum PostSaveActionType
  {
    /// <summary>
    /// No action.
    /// </summary>
    None,
    /// <summary>
    /// Also close the form.
    /// </summary>
    AndClose,
    /// <summary>
    /// Also create a new object.
    /// </summary>
    AndNew
  }
}