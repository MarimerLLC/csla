using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Xaml;
using cslalighttest.ViewModelTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ViewModelTests
{
  [TestClass]
  public class ViewModelTests
  {
    [TestMethod]
    public void ViewModel_DoAddNew_WithBindingListModel()
    {
      var viewModel = new TestViewModel<TestBindingList>();
      viewModel.Model = TestBindingList.NewEditableList();
      // List should be empty
      Assert.IsTrue(viewModel.Model.Count() == 0);
      viewModel.AddNew(this, new ExecuteEventArgs());
      Assert.IsTrue(viewModel.Model.Count() == 1);
      viewModel.AddNew(this, new ExecuteEventArgs());
      Assert.IsTrue(viewModel.Model.Count() == 2);

    }


    [TestMethod]
    public void ViewModel_DoAddNew_WithBusinessBindingListModel()
    {
      var viewModel = new TestViewModel<TestBusinessBindingList>();
      viewModel.Model = TestBusinessBindingList.NewEditableList();
      // List should be empty
      Assert.IsTrue(viewModel.Model.Count() == 0);
      viewModel.AddNew(this, new ExecuteEventArgs());
      Assert.IsTrue(viewModel.Model.Count() == 1);
      viewModel.AddNew(this, new ExecuteEventArgs());
      Assert.IsTrue(viewModel.Model.Count() == 2);

    }

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
