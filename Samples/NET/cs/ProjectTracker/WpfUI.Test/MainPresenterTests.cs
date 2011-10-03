using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bxf;

namespace WpfUI.Test
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class MainPresenterTests
  {
    private string _errorMessage;
    private string _errorTitle;
    private Status _status;
    private IView _view;
    private string _region;

    [TestInitialize]
    public void Initialize()
    {
      _errorMessage = null;
      _errorTitle = null;
      _status = null;
      _view = null;
      _region = null;

      var presenter = (IPresenter)Bxf.Shell.Instance;
      presenter.OnShowError += (message, title) =>
        {
          _errorMessage = message;
          _errorTitle = title;
        };
      presenter.OnShowStatus += (status) =>
        {
          _status = status;
        };
      presenter.OnShowView += (view, region) =>
        {
          _view = view;
          _region = region;
        };
    }

    [TestMethod]
    public void ShowMenu()
    {
      MainPresenter.ShowMenu();
      Assert.IsNull(_errorMessage);
      Assert.IsNull(_errorTitle);
      Assert.IsNull(_status);
      Assert.AreEqual(typeof(Views.MainMenu).AssemblyQualifiedName, _view.ViewName);
      Assert.AreEqual("mainMenuViewSource", _view.BindingResourceKey);
      Assert.IsInstanceOfType(_view.Model, typeof(ViewModels.MainMenu));
    }
  }
}
