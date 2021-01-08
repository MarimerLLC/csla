//-----------------------------------------------------------------------
// <copyright file="GrandChild.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Csla.Test.Basic
{
  [Serializable()]
  public class GrandChild : BusinessBase<GrandChild>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    internal static GrandChild NewGrandChild(string data)
    {
      GrandChild obj = new GrandChild();
      obj.Data = data;
      return obj;
    }

    internal static GrandChild GetGrandChild(IDataReader dr)
    {
      GrandChild obj = new GrandChild();
      obj.Fetch(dr);
      return obj;
    }

    public GrandChild()
    {
      MarkAsChild();
    }

    private void Fetch(IDataReader dr)
    {
      MarkOld();
    }

    internal void Update(IDbTransaction tr)
    {
      if (IsDeleted)
      {
        //we would delete here
        MarkNew();
      }
      else
      {
        if (IsNew)
        {
          //we would insert here
        }
        else
        {
          //we would update here
        }
        MarkOld();
      }
    }

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      base.OnDeserialized(context);
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Add("GCDeserialized", "GC Deserialized");
#pragma warning restore CS0618 // Type or member is obsolete
    }
  }
}