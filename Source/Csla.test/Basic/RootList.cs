//-----------------------------------------------------------------------
// <copyright file="RootList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace Csla.Test.Basic
{
  [Serializable]
  public class RootList : BusinessBindingListBase<RootList, RootListChild>
  {
    public RootList()
    {
      AllowEdit = true;
      AllowNew = true;
      AllowRemove = true;
    }

    protected override object AddNewCore()
    {
      RootListChild child = new RootListChild();
      Add(child);
      return child;
    }
  }

  [Serializable]
  public class RootListChild : BusinessBase<RootListChild>
  {
    public static PropertyInfo<int> DataProperty = RegisterProperty<int>(c => c.Data);
    public int Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public string[] GetRuleDescriptions()
    {
      string[] result = BusinessRules.GetRuleDescriptions();
      if (result == null)
        result = new string[] { };
      return result;
    }

    public RootListChild()
    {
      MarkAsChild();
    }
  }
}