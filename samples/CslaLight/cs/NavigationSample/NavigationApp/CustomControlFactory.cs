using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace NavigationApp
{
  public class CustomControlFactory : Csla.Silverlight.IControlNameFactory
  {

    #region IControlNameFactory Members

    public Control ControlNameToControl(string controlName)
    {
      if (controlName.ToUpper().Contains("_1".ToUpper()))
      {
        return new ControlOne();
      }
      else if (controlName.ToUpper().Contains("_2".ToUpper()))
      {
        return new ControlTwo();
      }
      else if (controlName.ToUpper().Contains("_5".ToUpper()))
      {
        return new ControlFive();
      }
      else
        return null;
    }

    public string ControlToControlName(Control control)
    {
      if (control is ControlOne)
        return "Control_1";
      if (control is ControlTwo)
        return "Control_2";
      if (control is ControlFive)
        return "Control_5";
      return control.GetType().AssemblyQualifiedName;
    }

    #endregion
  }
}
