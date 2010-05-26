using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Bxf
{
  /// <summary>
  /// Defines the members for a view
  /// factory object.
  /// </summary>
  public interface IViewFactory
  {
    /// <summary>
    /// Creates a populated IView object, including
    /// the instance of the view.
    /// </summary>
    /// <param name="viewName">Name of the view.</param>
    /// <param name="bindingResourceKey">Name of the binding resource.</param>
    /// <param name="model">Reference to the model or viewmodel for the view.</param>
    IView GetView(string viewName, string bindingResourceKey, object model);   
  }
}
