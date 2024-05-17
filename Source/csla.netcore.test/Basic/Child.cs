//-----------------------------------------------------------------------
// <copyright file="Child.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Data;

namespace Csla.Test.Basic
{
  [Serializable]
  public class Child : BusinessBase<Child>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    public string Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    public Guid Guid { get; } = System.Guid.NewGuid();

    public static readonly PropertyInfo<GrandChildren> GrandChildrenProperty = RegisterProperty<GrandChildren>(c => c.GrandChildren);
    public GrandChildren GrandChildren
    {
      get => GetProperty(GrandChildrenProperty);
      private set => LoadProperty(GrandChildrenProperty, value);
    }

    [CreateChild]
    private void Create([Inject] IChildDataPortal<GrandChildren> dataPortal)
    {
      GrandChildren = dataPortal.CreateChild();
    }

    [FetchChild]
    private void Fetch(IDataReader dr, [Inject] IChildDataPortal<GrandChildren> dataPortal)
    {
      GrandChildren = dataPortal.CreateChild();
      MarkOld();
    }

    [Update]
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