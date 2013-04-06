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
    /// Perform a cancel.
    /// </summary>
    Cancel,
    /// <summary>
    /// Close the form.
    /// </summary>
    Close
  }

  /// <summary>
  /// The possible options for posting
  /// and saving.
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
