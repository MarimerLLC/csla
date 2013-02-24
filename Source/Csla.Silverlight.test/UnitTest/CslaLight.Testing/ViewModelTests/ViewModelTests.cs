using System;
using System.Diagnostics;
using System.Linq;
using Csla;
using Csla.DataPortalClient;
using Csla.Xaml;
using UnitDriven;

namespace cslalighttest.ViewModelTests
{
  [TestClass]
  public class ViewModelTests : TestBase
  {
    [TestMethod]
    public void ViewModel_DoAddNew_WithBindingListModel()
    {
      try
      {
        using (var context = GetContext())
        {
          DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
          WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
          TestBindingList.NewEditableList((o, e) =>
                                            {
                                              if (e.Error != null)
                                              {
                                                Debug.WriteLine(e.Error.ToString());
                                                context.Assert.Fail(e.Error.Message);
                                              }
                                              else
                                              {
                                                try
                                                {
                                                  var viewModel = new TestViewModel<TestBindingList>();
                                                  viewModel.Model = e.Object;

                                                  // List should be empty
                                                  context.Assert.IsTrue(viewModel.Model.Count() == 0);
                                                  viewModel.AddNew(this, new ExecuteEventArgs());
                                                  context.Assert.IsTrue(viewModel.Model.Count() == 1);
                                                  viewModel.AddNew(this, new ExecuteEventArgs());
                                                  context.Assert.IsTrue(viewModel.Model.Count() == 2);
                                                  context.Assert.Success();
                                                }
                                                catch (Exception ex)
                                                {
                                                  Debug.WriteLine(ex.ToString());
                                                  throw;
                                                }
                                              }

                                            });

          context.Complete();
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.ToString());
      }
    }


    [TestMethod]
    public void ViewModel_DoAddNew_WithBusinessBindingListModel()
    {
      try
      {
        using (var context = GetContext())
        {

          DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy).AssemblyQualifiedName;
          WcfProxy.DefaultUrl = cslalighttest.Properties.Resources.RemotePortalUrl;
          TestBusinessBindingList.NewEditableList((o, e) =>
                                                    {
                                                      if (e.Error != null)
                                                      {
                                                        Debug.WriteLine(e.Error.ToString());
                                                        context.Assert.Fail(e.Error.Message);
                                                      }
                                                      else
                                                      {
                                                        var viewModel = new TestViewModel<TestBusinessBindingList>();
                                                        viewModel.Model = e.Object;

                                                        // List should be empty
                                                        context.Assert.IsTrue(viewModel.Model.Count() == 0);
                                                        viewModel.AddNew(this, new ExecuteEventArgs());
                                                        context.Assert.IsTrue(viewModel.Model.Count() == 1);
                                                        viewModel.AddNew(this, new ExecuteEventArgs());
                                                        context.Assert.IsTrue(viewModel.Model.Count() == 2);
                                                        context.Assert.Success();
                                                      }
                                                    });

          context.Complete();
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.ToString());
      }
    }

  }
}
