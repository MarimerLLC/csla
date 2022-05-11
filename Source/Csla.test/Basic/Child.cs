//-----------------------------------------------------------------------
// <copyright file="Child.cs" company="Marimer LLC">
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
  public class Child : BusinessBase<Child>
  {
    public static PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));
    public static PropertyInfo<Guid> GuidProperty = RegisterProperty<Guid>(nameof(Guid));
    public static PropertyInfo<GrandChildren> GrandChildrenProperty = RegisterProperty<GrandChildren>(c => c.GrandChildren);

    protected override object GetIdValue()
    {
      return ReadProperty(DataProperty);
    }

    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public override bool Equals(object obj)
    {
      if (obj == null || !(obj is Child))
      {
        return false;
      }

      return ReadProperty(DataProperty) == ((Child)(obj)).ReadProperty(DataProperty);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public Guid Guid
    {
      get { return ReadProperty(GuidProperty); }
    }

    public GrandChildren GrandChildren
    {
      get { return GetProperty(GrandChildrenProperty); }
    }

    internal static Child NewChild(IDataPortal<Child> dataPortal, string data)
    {
      return dataPortal.Create(data);
    }

    internal static Child GetChild(IDataPortal<Child> dataPortal, IDataReader dr)
    {
      Child obj;
      obj = dataPortal.Fetch(dr);
      return obj;
    }

    public Child()
    {
      MarkAsChild();
    }

    [Create]
    [CreateChild]
    private void Create(string data, [Inject] IChildDataPortal<GrandChildren> childDataPortal)
    {
      LoadProperty(GuidProperty, Guid.NewGuid());
      LoadProperty(DataProperty, data);
      LoadProperty(GrandChildrenProperty, childDataPortal.CreateChild());
    }

    [Fetch]
    [FetchChild]
    private void Fetch(IDataReader dr, [Inject] IChildDataPortal<GrandChildren> childDataPortal)
    {
      LoadProperty(GrandChildrenProperty, childDataPortal.CreateChild());
    }

    [UpdateChild]
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
  }
}