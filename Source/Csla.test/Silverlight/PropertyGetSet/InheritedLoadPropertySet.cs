//-----------------------------------------------------------------------
// <copyright file="InheritedLoadPropertySet.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Test.Silverlight.PropertyGetSet
{
  [Serializable]
  public class InheritedLoadPropertySet : AbstractGetSet<InheritedLoadPropertySet>
  {
    private static int _forceLoad;

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