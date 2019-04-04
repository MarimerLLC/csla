//-----------------------------------------------------------------------
// <copyright file="InheritedLoadPropertySet.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Test.Silverlight.PropertyGetSet
{
  [Serializable]
  public class InheritedLoadPropertySet : AbstractGetSet<InheritedLoadPropertySet>
  {
#pragma warning disable CS0414
    private static int _forceLoad;
#pragma warning restore CS0414

    public InheritedLoadPropertySet()
    {
      _forceLoad = 0;
      //Always call DataPortal_Insert on save
      MarkNew();
    }
    //public void LoadIdProperty(int newIdValue)
    //{
    //  LoadProperty(IdProperty, newIdValue);
    //}

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      _forceLoad = 0;
      base.OnDeserialized(context);
    }

    protected override void DataPortal_Insert()
    {
      LoadProperty(IdProperty, 1);
    }
  }
}