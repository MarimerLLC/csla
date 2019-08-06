//-----------------------------------------------------------------------
// <copyright file="BypassBusinessBaseUsingFactory.cs" company="Marimer LLC">
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
  [Csla.Server.ObjectFactory("Csla.Test.BypassPropertyChecks.TestObjectFactory,Csla.Test")]
  public class BypassBusinessBaseUsingFactory : BusinessBase<BypassBusinessBaseUsingFactory>
  {
    internal BypassBusinessBaseUsingFactory()
    {
      MarkOld();
    }

    public static BypassBusinessBaseUsingFactory GetObject()
    {
      return Csla.DataPortal.Fetch<BypassBusinessBaseUsingFactory>();
    }

    protected static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id, "Id");
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value); }
    }

    protected static PropertyInfo<int> Id2Property = RegisterProperty<int>(c => c.Id2, "Id2");
    public int Id2
    {
      get { return GetProperty<int>(Id2Property); }
      set { SetProperty<int>(Id2Property, value); }
    }

    private int _id3;
    protected static PropertyInfo<int> Id3Property = RegisterProperty<int>(c => c.Id3, "Id3");
    public int Id3
    {
      get { return GetProperty<int>(Id3Property, _id3); }
      set { SetProperty(Id3Property, ref _id3, value); }
    }

    private int _id4;
    protected static PropertyInfo<int> Id4Property = RegisterProperty<int>(c => c.Id4, "Id4");
    public int Id4
    {
      get { return GetProperty<int>(Id4Property, _id4); }
      set { SetProperty(Id4Property, ref _id4, value); }
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