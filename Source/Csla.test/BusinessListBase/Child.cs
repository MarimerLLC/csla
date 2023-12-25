//-----------------------------------------------------------------------
// <copyright file="Child.cs" company="Marimer LLC">
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

namespace Csla.Test.BusinessListBase
{
  [Serializable]
  public class Child : BusinessBase<Child>
  {
    private static PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    [CreateChild]
    private void CreateChild() { }

    [InsertChild]
    private void Child_Insert()
    { }

    [UpdateChild]
    private void Child_Update()
    { }

    [DeleteSelfChild]
    private void Child_DeleteSelf()
    { }
  }
}