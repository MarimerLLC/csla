//-----------------------------------------------------------------------
// <copyright file="ContainsList.cs" company="Marimer LLC">
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
    private void Fetch(bool child, [Inject] IChildDataPortal<SingleList> childDataPortal)
    {
      LoadProperty(ListProperty, childDataPortal.FetchChild(true));
      if (child)
        MarkAsChild();
    }
  }
}