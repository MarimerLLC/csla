//-----------------------------------------------------------------------
// <copyright file="MockEditableChild.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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

    private static PropertyInfo<GrandChildList> GrandChildrenProperty = RegisterProperty<GrandChildList>(
      typeof(MockEditableChild),
      new PropertyInfo<GrandChildList>("GrandChildren"));

    private static PropertyInfo<string> DataPortalMethodProperty = RegisterProperty(
      typeof(MockEditableChild),
      new PropertyInfo<string>("DataPortalMethod"));

    public string DataPortalMethod
    {
      get { return GetProperty(DataPortalMethodProperty); }
      set { SetProperty(DataPortalMethodProperty, value); }
    }

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

    public GrandChildList GrandChildren
    {
      get { return GetProperty<GrandChildList>(GrandChildrenProperty); }
      set { SetProperty<GrandChildList>(GrandChildrenProperty, value); }
    }

    #endregion

    public override string ToString()
    {
      return Name.ToString();
    }
  }
}