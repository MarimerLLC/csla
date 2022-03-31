//-----------------------------------------------------------------------
// <copyright file="AParent.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.LazyLoad
{
  [Serializable]
  public class AParent : Csla.BusinessBase<AParent>
  {
    public static PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(c => c.Id);
    public Guid Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<AChildList> ChildListProperty = 
      RegisterProperty<AChildList>(c => c.ChildList, "Child list", null, RelationshipTypes.LazyLoad);
    public AChildList ChildList
    {
      get 
      {
        return LazyGetProperty(ChildListProperty, FetchChildList);
      }
    }

    public AChildList GetChildList()
    {
      if (FieldManager.FieldExists(ChildListProperty))
        return ReadProperty<AChildList>(ChildListProperty);
      else
        return null;
    }

    public new int EditLevel
    {
      get { return base.EditLevel; }
    }

    [Create]
    private void Create()
    {
      using (BypassPropertyChecks)
        Id = Guid.NewGuid();
    }

    private AChildList FetchChildList()
    {
      return GetDataPortal<AChildList>().Fetch();
    }
  }
}