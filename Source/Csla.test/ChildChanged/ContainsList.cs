//-----------------------------------------------------------------------
// <copyright file="ContainsList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Test.ChildChanged
{
  [Serializable]
  public class ContainsList : BusinessBase<ContainsList>
  {
    public ContainsList()
    { }

    public ContainsList(bool child)
    {
      if (child)
        MarkAsChild();
    }

    private static PropertyInfo<SingleList> ListProperty = RegisterProperty(new PropertyInfo<SingleList>("List"));
    public SingleList List
    {
      get 
      {
        return GetProperty(ListProperty); 
      }
    }

    [Fetch]
    private void Fetch([Inject] IChildDataPortal<SingleList> childDataPortal)
    {
      LoadProperty(ListProperty, childDataPortal.FetchChild(true));
    }

    [Fetch]
    [FetchChild]
    private void Fetch(bool child, [Inject] IChildDataPortal<SingleList> childDataPortal)
    {
      LoadProperty(ListProperty, childDataPortal.FetchChild(true));
      if (child)
        MarkAsChild();
    }
  }
}