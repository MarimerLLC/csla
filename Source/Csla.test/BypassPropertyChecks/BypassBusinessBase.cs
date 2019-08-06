//-----------------------------------------------------------------------
// <copyright file="BypassBusinessBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization;

namespace Csla.Test.BypassPropertyChecks
{
  [Serializable()]
  public class BypassBusinessBase : BusinessBase<BypassBusinessBase>
  {
    public BypassBusinessBase()
    {
      MarkOld();
    }

    protected static PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id), "Id");
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value, Csla.Security.NoAccessBehavior.ThrowException); }
    }

    protected static PropertyInfo<int> Id2Property = RegisterProperty<int>(c => c.Id2, "Id2");
    public int Id2
    {
      get { return GetProperty<int>(Id2Property); }
      set { SetProperty<int>(Id2Property, value, Csla.Security.NoAccessBehavior.ThrowException); }
    }

    private int _id3;
    protected static PropertyInfo<int> Id3Property = RegisterProperty<int>(c => c.Id3, "Id3", 0, RelationshipTypes.PrivateField);
    public int Id3
    {
      get { return GetProperty<int>(Id3Property, _id3); }
      set { SetProperty<int>(Id3Property.Name, ref _id3, value, Csla.Security.NoAccessBehavior.ThrowException); }
    }

    private int _id4;
    protected static PropertyInfo<int> Id4Property = RegisterProperty<int>(c => c.Id4, "Id4", 0, RelationshipTypes.PrivateField);
    public int Id4
    {
      get { return GetProperty<int>(Id4Property, _id4); }
      set { SetProperty<int>(Id4Property.Name, ref _id4, value, Csla.Security.NoAccessBehavior.ThrowException); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.ReadProperty, IdProperty, new List<string> { "Admin" }));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.WriteProperty, IdProperty, new List<string> { "Admin" }));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.ReadProperty, Id2Property, new List<string> { "Random" }));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.WriteProperty, Id2Property, new List<string> { "Random" }));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.ReadProperty, Id3Property, new List<string> { "Admin" }));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.WriteProperty, Id3Property, new List<string> { "Admin" }));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.ReadProperty, Id4Property, new List<string> { "Random" }));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.WriteProperty, Id4Property, new List<string> { "Random" }));
    }

    public void LoadIdByPass(int id)
    {
      using (this.BypassPropertyChecks)
      {
        Id = id;
      }
    }

    public void LoadIdByNestedPass(int id)
    {
      using (this.BypassPropertyChecks)
      {
        using (this.BypassPropertyChecks)
        {
          
        }
        // must still be in bypass property checks
        Id = id;
      }
    }

    public void LoadId(int id)
    {
      Id = id;
    }

    public int ReadIdByPass()
    {
      using (this.BypassPropertyChecks)
      {
        return Id;
      }
    }

    public int ReadId()
    {
      return Id;
    }

    public void LoadId2ByPass(int id)
    {
      using (this.BypassPropertyChecks)
      {
        Id2 = id;
      }
    }

    public void LoadId2(int id)
    {
      Id2 = id;
    }

    public int ReadId2ByPass()
    {
      using (this.BypassPropertyChecks)
      {
        return Id2;
      }
    }

    public int ReadId2()
    {
      return Id2;
    }


    public void LoadId3ByPass(int id)
    {
      using (this.BypassPropertyChecks)
      {
        Id3 = id;
      }
    }

    public void LoadId3(int id)
    {
      Id3 = id;
    }

    public int ReadId3ByPass()
    {
      using (this.BypassPropertyChecks)
      {
        return Id3;
      }
    }

    public int ReadId3()
    {
      return Id3;
    }

    public void LoadId4ByPass(int id)
    {
      using (this.BypassPropertyChecks)
      {
        Id4 = id;
      }
    }

    public void LoadId4(int id)
    {
      Id4 = id;
    }

    public int ReadId4ByPass()
    {
      using (this.BypassPropertyChecks)
      {
        return Id4;
      }
    }

    public int ReadId4()
    {
      return Id4;
    }

  }
}