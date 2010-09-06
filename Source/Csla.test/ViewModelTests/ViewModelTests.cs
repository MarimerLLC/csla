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
  }
}
