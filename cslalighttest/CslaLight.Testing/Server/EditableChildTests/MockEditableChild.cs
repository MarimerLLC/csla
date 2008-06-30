using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;

namespace Csla.Testing.Business.EditableChildTests
{
  [Serializable]
  public partial class MockEditableChild : BusinessBase<MockEditableChild>
  {
    #region  Properties

    private static PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(
      typeof(MockEditableChild),
      new PropertyInfo<Guid>("Id"));

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(
      typeof(MockEditableChild),
      new PropertyInfo<string>("Name"));

    private static PropertyInfo<string> DataPortalMethodProperty = RegisterProperty<string>(
      typeof(MockEditableChild),
      new PropertyInfo<string>("DataPortalMethod"));

    public Guid Id
    {
      get { return GetProperty<Guid>(IdProperty); }
      set { SetProperty<Guid>(IdProperty, value); }
    }

    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    public string DataPortalMethod
    {
      get { return GetProperty<string>(DataPortalMethodProperty); }
      set { SetProperty<string>(DataPortalMethodProperty, value); }
    }

    public override string ToString()
    {
      return Name.ToString();
    }

    #endregion
  }
}
