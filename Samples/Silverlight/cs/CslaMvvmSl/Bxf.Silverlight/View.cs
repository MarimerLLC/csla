using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Bxf
{
  /// <summary>
  /// Contains the information about a view
  /// necessary for Bxf to initialize and display
  /// the view.
  /// </summary>
  public class View : IView
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="viewName">Name of the view.</param>
    /// <param name="view">Instance of the view.</param>
    /// <param name="bindingResourceKey">Name of the binding resource.</param>
    /// <param name="model">Instance of the model or viewmodel for the view.</param>
    public View(string viewName, UserControl view, string bindingResourceKey, object model)
    {
      ViewInstance = view;
      ViewName = viewName;
      BindingResourceKey = bindingResourceKey;
      Model = model;
    }

    /// <summary>
    /// Gets the instance of the view.
    /// </summary>
    public UserControl ViewInstance { get; private set; }
    /// <summary>
    /// Gets the name of the view used to create
    /// the view instance by the view factory.
    /// </summary>
    public string ViewName { get; private set; }
    /// <summary>
    /// Gets the name of the binding resource to which
    /// the model should be connected.
    /// </summary>
    public string BindingResourceKey { get; private set; }
    /// <summary>
    /// Gets the instance of the model 
    /// or viewmodel for the view.
    /// </summary>
    public object Model { get; private set; }
  }
}
