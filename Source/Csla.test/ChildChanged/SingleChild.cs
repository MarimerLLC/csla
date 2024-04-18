﻿//-----------------------------------------------------------------------
// <copyright file="SingleChild.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Test.ChildChanged
{
  [Serializable]
  public class SingleChild : BusinessBase<SingleChild>
  {
    public SingleChild()
    { }

    public SingleChild(bool child)
    {
      if (child)
        MarkAsChild();
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty(new PropertyInfo<string>("Name"));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static PropertyInfo<SingleRoot> ChildProperty = RegisterProperty(new PropertyInfo<SingleRoot>("Child"));
    public SingleRoot Child
    {
      get 
      {
        return GetProperty(ChildProperty); 
      }
    }

    [Fetch]
    [FetchChild]
    private void Fetch(bool child, [Inject] IChildDataPortal<SingleRoot> childDataPortal)
    {
      LoadProperty(ChildProperty, childDataPortal.FetchChild(true));
      if (child)
        MarkAsChild();
    }
  }
}