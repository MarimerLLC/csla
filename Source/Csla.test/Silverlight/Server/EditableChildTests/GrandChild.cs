//-----------------------------------------------------------------------
// <copyright file="GrandChild.cs" company="Marimer LLC">
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
  public partial class GrandChild : BusinessBase<GrandChild>
  {
    #region Properties

    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(
      typeof(GrandChild),
      new PropertyInfo<int>("Id"));

    public static PropertyInfo<Guid> ParentIdProperty = RegisterProperty<Guid>(
      typeof(GrandChild),
      new PropertyInfo<Guid>("ParentId"));

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(
      typeof(GrandChild),
      new PropertyInfo<string>("Name"));

    private static PropertyInfo<string> DataPortalMethodProperty = RegisterProperty(
      typeof(GrandChild),
      new PropertyInfo<string>("DataPortalMethod"));

    public string DataPortalMethod
    {
      get { return GetProperty(DataPortalMethodProperty); }
      set { SetProperty(DataPortalMethodProperty, value); }
    }

    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value); }
    }

    public Guid ParentId
    {
      get { return GetProperty<Guid>(ParentIdProperty); }
      set { SetProperty<Guid>(ParentIdProperty, value); }
    }

    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    #endregion

    protected override void DataPortal_Update()
    {
      LoadProperty<string>(DataPortalMethodProperty, "DataPortal_Update");
    }
  }
}