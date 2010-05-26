using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Bxf
{
  /// <summary>
  /// Defines the members of a shell manager
  /// object for use with the Bxf framework.
  /// </summary>
  public interface IShell
  {
    /// <summary>
    /// Gets or sets a reference to the view factory
    /// object for this presenter.
    /// </summary>
    IViewFactory ViewFactory { get; set; }
    /// <summary>
    /// Initializes the binding resource and raises
    /// displays the view.
    /// </summary>
    /// <param name="view">View to show.</param>
    /// <param name="region">UI region where view should be displayed.</param>
    void ShowView(IView view, string region);
    /// <summary>
    /// Initializes the binding resource and raises
    /// displays the view.
    /// </summary>
    /// <param name="viewName">Name of the view.</param>
    /// <param name="bindingResourceKey">Name of the binding resource
    /// key to which the model should be connected.</param>
    /// <param name="model">Model or viewmodel to connect to the
    /// binding resource.</param>
    /// <param name="region">UI region where view should be displayed.</param>
    void ShowView(string viewName, string bindingResourceKey, object model, string region);
    /// <summary>
    /// Displays the error message.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="title">Error title.</param>
    void ShowError(string message, string title);
    /// <summary>
    /// Displays the status text.
    /// </summary>
    /// <param name="status">Status text.</param>
    void ShowStatus(Status status);
    /// <summary>
    /// Indicates that a new IPrincipal object
    /// has been set.
    /// </summary>
    void NewUser();
  }
}
