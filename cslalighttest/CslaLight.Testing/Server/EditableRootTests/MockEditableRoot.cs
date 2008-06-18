using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Csla.Serialization;

namespace Csla.Testing.Business.EditableRootTests
{
  [Serializable]
  public partial class MockEditableRoot : BusinessBase<MockEditableRoot>
  {
    public static readonly Guid MockEditableRootId = new Guid("{7E7127CF-F22C-4903-BDCE-1714C81A9E89}");

    #region  Business Methods

    private static PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(
      typeof(MockEditableRoot), 
      new PropertyInfo<Guid>("Id"));

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(
      typeof(MockEditableRoot), 
      new PropertyInfo<string>("Name"));

    public Guid Id
    {
      get { return GetProperty<Guid>(IdProperty); }
    }

    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    public override string ToString()
    {
      return Id.ToString();
    }

    #endregion

    #region  Validation Rules

    //protected override void AddBusinessRules()
    //{
    //  // TODO: add business rules
    //}

    #endregion

    #region  Authorization Rules

    //protected override void AddAuthorizationRules()
    //{
    //  // add AuthorizationRules here
    //}

    //protected static void AddObjectAuthorizationRules()
    //{
    //  // add object-level authorization rules here
    //}

    #endregion
  }
}
