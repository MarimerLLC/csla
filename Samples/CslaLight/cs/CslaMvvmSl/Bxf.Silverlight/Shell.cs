using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;

namespace Bxf
{
  /// <summary>
  /// Default presenter used to display view, error
  /// and status information.
  /// </summary>
  public class Shell : IShell, IPresenter
  {
    #region Instance property

    private static IShell _presenter = new Shell();

    /// <summary>
    /// Gets or sets the current instance of 
    /// presenter object.
    /// </summary>
    public static IShell Instance
    {
      get { return _presenter; }
      set { _presenter = value; }
    }

    #endregion

    #region ViewFactory

    private IViewFactory _viewFactory = new ViewFactory();

    /// <summary>
    /// Gets or sets a reference to the view factory
    /// object for this presenter.
    /// </summary>
    public IViewFactory ViewFactory
    {
      get { return _viewFactory; }
      set { _viewFactory = value; }
    }

    #endregion

    #region IPresenter

    /// <summary>
    /// Event raised when a view is to be displayed.
    /// </summary>
    public event Action<IView, string> OnShowView;
    /// <summary>
    /// Event raised when an error is to be displayed.
    /// </summary>
    public event Action<string, string> OnShowError;
    /// <summary>
    /// Event raised when a new status is to be displayed.
    /// </summary>
    public event Action<Status> OnShowStatus;
    /// <summary>
    /// Event raised when a new IPrincipal object
    /// has been set.
    /// </summary>
    public event Action OnNewUser;

    #endregion

    #region IShell

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
    public void ShowView(string viewName, string bindingResourceKey, object model, string region)
    {
      ShowView(ViewFactory.GetView(viewName, bindingResourceKey, model), region);
    }

    /// <summary>
    /// Initializes the binding resource and raises
    /// displays the view.
    /// </summary>
    /// <param name="view">View to show.</param>
    /// <param name="region">UI region where view should be displayed.</param>
    public virtual void ShowView(IView view, string region)
    {
      if (view == null)
        view = ViewFactory.GetView(null, null, null);

      InitializeBindingResource(view);

      if (OnShowView != null)
        OnShowView(view, region);
    }

    /// <summary>
    /// Connects the model to the binding resource
    /// in the view.
    /// </summary>
    /// <param name="view">IView object containing information
    /// about the view, binding resource and model.</param>
    /// <returns>Updated view instance.</returns>
    protected virtual void InitializeBindingResource(IView view)
    {
      var form = view.ViewInstance;
      if (form != null && !string.IsNullOrEmpty(view.BindingResourceKey))
      {
        var resource = form.Resources[view.BindingResourceKey];
        if (resource == null)
          throw new NotSupportedException("Binding resource key not found in view resources");

        var viewsource = resource as System.Windows.Data.CollectionViewSource;
        if (viewsource != null)
        {
          // make sure model is a list (or wrapped in one)
          var list = view.Model as System.Collections.IEnumerable;
          if (list == null)
            list = new List<object>() { view.Model };
          viewsource.Source = list;
        }
        else
        {
          var viewmodel = resource as Csla.Xaml.IViewModel;
          if (viewmodel != null)
            viewmodel.Model = view.Model;
          else
          {
            form.Resources.Remove(view.BindingResourceKey);
            form.Resources.Add(view.BindingResourceKey, view.Model);
          }
        }
      }
    }

    /// <summary>
    /// Displays the error message.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="title">Error title.</param>
    public virtual void ShowError(string message, string title)
    {
      if (OnShowError != null)
        OnShowError(message, title);
    }

    /// <summary>
    /// Displays the status text.
    /// </summary>
    /// <param name="status">Status text.</param>
    public virtual void ShowStatus(Status status)
    {
      if (OnShowStatus != null)
        OnShowStatus(status);
    }

    /// <summary>
    /// Indicates that a new IPrincipal object
    /// has been set.
    /// </summary>
    public void NewUser()
    {
      if (OnNewUser != null)
        OnNewUser();
    }

    #endregion
  }
}
