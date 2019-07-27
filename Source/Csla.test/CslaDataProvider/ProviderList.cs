//-----------------------------------------------------------------------
// <copyright file="ProviderList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla;
namespace Csla.Test.CslaDataProvider
{
  [Serializable]
  public class ProviderList : BusinessBindingListBase<ProviderList, ProviderChild>
  {
    public ProviderList()
    {
      AllowEdit = true;
      AllowNew = true;
      AllowRemove = true;
    }

    public static ProviderList GetList()
    {
      ProviderList returnValue = new ProviderList();
      returnValue.Add(new ProviderChild(true));
      returnValue.Add(new ProviderChild(true));
      return returnValue;
    }

    protected override object AddNewCore()
    {
      ProviderChild child = new ProviderChild();
      Add(child);
      return child;
    }

    //error testing
    public new ProviderList Save()
    {
      throw new Exception();
    }

  }

  [Serializable]
  public class ProviderChild : BusinessBase<ProviderChild>
  {
    public static PropertyInfo<int> DataProperty = RegisterProperty<int>(c => c.Data);
    public int Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public ProviderChild()
    {
      MarkAsChild();
    }

    public ProviderChild(bool makeOld)
    {
      MarkAsChild();
      if (makeOld)
        MarkOld();
    }

    public void MarkItOld()
    {
      MarkOld();
    }
  }
}