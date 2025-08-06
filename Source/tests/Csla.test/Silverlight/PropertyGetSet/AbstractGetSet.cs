//-----------------------------------------------------------------------
// <copyright file="AbstractGetSet.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Test.Silverlight.PropertyGetSet
{
  [Serializable]
  public class AbstractGetSet<T> : BusinessBase<T> where T : AbstractGetSet<T>
  {
    protected static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id, "Id");
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value); }
    }

#pragma warning disable CS0414
    private static int _forceLoad;
#pragma warning restore CS0414

    protected AbstractGetSet()
    {
      _forceLoad = 0;
    }

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      _forceLoad = 0;
      base.OnDeserialized(context);
    }
  }
}