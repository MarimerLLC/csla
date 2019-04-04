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
using Csla;
using Csla.Serialization;

namespace Csla.Test.ChildChanged
{
  [Serializable]
  public class Grandchild : BusinessBase<Grandchild>
  {
    private static PropertyInfo<string> NameProperty = RegisterProperty(new PropertyInfo<string>("Name"));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static PropertyInfo<SingleChild> ChildProperty = RegisterProperty(new PropertyInfo<SingleChild>("Child"));
    public SingleChild Child
    {
      get
      {
        if (!FieldManager.FieldExists(ChildProperty))
          LoadProperty(ChildProperty, new SingleChild(true));
        return GetProperty(ChildProperty);
      }
    }
  }
}