//-----------------------------------------------------------------------
// <copyright file="BadGetSet.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.PropertyGetSet
{
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  [Serializable]
  public class BadGetSet : BusinessBase<BadGetSet>
  {
    // the registering class is intentionally incorrect for this test
    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(typeof(EditableGetSet), new PropertyInfo<int>("Id"));
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value); }
    }
  }

#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  [Serializable]
  public class BadGetSetTwo : BusinessBase<BadGetSetTwo>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value); }
    }

    // the registering class is intentionally incorrect for this test
    public static readonly PropertyInfo<int> IdTwoProperty = RegisterProperty<int>(c => c.Id);
    public int IdTwo
    {
      get { return GetProperty<int>(IdTwoProperty); }
      set { SetProperty<int>(IdTwoProperty, value); }
    }
  }
}