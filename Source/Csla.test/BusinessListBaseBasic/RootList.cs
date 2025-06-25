//-----------------------------------------------------------------------
// <copyright file="RootList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.BusinessListBaseBasic
{
  [Serializable]
  public class RootList : BusinessListBase<RootList, RootListChild>
  {

    [Create, RunLocal]
    private void Create()
    {
      AllowEdit = true;
      AllowNew = true;
      AllowRemove = true;
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
        result = [];
      return result;
    }

    public RootListChild()
    {
      MarkAsChild();
    }
  }
}