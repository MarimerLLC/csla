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
using UnitDriven;

namespace cslalighttest.ViewModelTests
{
  [TestClass]
  public class ViewModelTestsSync
  {

    [TestMethod]
    public void ViewModel_CheckAccess_WithNoListModel()
    {
      var viewModel = new TestViewModel<TestBusinessBindingList>();

      // unless otherwise set all object level permissions are true
      Assert.IsTrue(viewModel.CanCreateObject);
      Assert.IsTrue(viewModel.CanGetObject);
      Assert.IsTrue(viewModel.CanDeleteObject);
      Assert.IsTrue(viewModel.CanEditObject);

      // when no Model is set then instance level CanCreate and CanFetch should be the same as object level permissions
      Assert.IsTrue(viewModel.CanCreate);
      Assert.IsTrue(viewModel.CanFetch);
      Assert.IsFalse(viewModel.CanSave);
      Assert.IsFalse(viewModel.CanDelete);
    }
  }
}
