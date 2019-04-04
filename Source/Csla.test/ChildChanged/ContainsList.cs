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
        if (!FieldManager.FieldExists(ListProperty))
          LoadProperty(ListProperty, new SingleList(true));
        return GetProperty(ListProperty); 
      }
    }
  }
}