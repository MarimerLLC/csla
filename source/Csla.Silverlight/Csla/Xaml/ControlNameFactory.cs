using System;
using System.Windows;
using System.Windows.Controls;

namespace Csla.Xaml
{
  /// <summary>
  /// Maps controls to names and names to controls.
  /// </summary>
  public class ControlNameFactory : IControlNameFactory
  {
    #region IControlNameFactory Members

    /// <summary>
    /// Convert full name of the control to name that will be used for 
    /// creation of bookmakrs
    /// </summary>
    /// <param name="control">
    /// User Control or Control object that can be used
    /// by navigator as a target.
    /// </param>
    /// <returns>Short name of control used for bookmarks</returns>
    public string ControlToControlName(Control control)
    {
      return control.GetType().AssemblyQualifiedName;
    }

    /// <summary>
    /// Convert short name of control used for bookmarks to
    /// User Control or Control object
    /// </summary>
    /// <param name="controlName">Short name of control used for bookmarks</param>
    /// <returns>User Control or Control object</returns>
    public Control ControlNameToControl(string controlName)
    {
      Type controlType = Csla.Reflection.MethodCaller.GetType(controlName);
      if (controlType != null)
        return (Control)Activator.CreateInstance(controlType);
      else
        return null;
    }

    #endregion
  }
}
