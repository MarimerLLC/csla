using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla.DataPortalClient;

namespace Csla.Testing.Business.EditableChildTests
{
  public partial class MockList
  {    
    #region  Factory Methods

    public static void FetchAll(Action<MockList, Exception> completed)
    {
      DataPortal<MockList> dp = new DataPortal<MockList>();
      dp.FetchCompleted += (o, e) => { completed(e.Object, e.Error); };
      dp.BeginFetch();
    }

    public static void FetchByName(string name, Action<MockList, Exception> completed)
    {
      DataPortal<MockList> dp = new DataPortal<MockList>();
      dp.FetchCompleted += (o, e) => { completed(e.Object, e.Error);};
      dp.BeginFetch(new SingleCriteria<MockList, string>(name));
    }

    #endregion

    #region  Data Access

    public void DataPortal_Fetch(LocalProxy<MockList>.CompletedHandler completed)
    {
      // fetch with no filter
      Fetch(completed, "");
    }

    public void DataPortal_Fetch(LocalProxy<MockList>.CompletedHandler completed, SingleCriteria<MockList, string> criteria)
    {
      Fetch(completed, criteria.Value);
    }

    #endregion
  }
}
