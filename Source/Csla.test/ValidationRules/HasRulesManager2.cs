//-----------------------------------------------------------------------
// <copyright file="HasRulesManager2.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core;

namespace Csla.Test.ValidationRules
{
  [Serializable()]
  public partial class HasRulesManager2 : BusinessBase<HasRulesManager2>
  {
    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(NameProperty, 10));
    }

    [Serializable()]
    public class Criteria : CriteriaBase<Criteria>
    {
      public string _name;

      public Criteria()
      {
        _name = "<new>";
      }

      public Criteria(string name)
      {
        this._name = name;
      }

      protected override void OnGetState(SerializationInfo info, StateMode mode)
      {
        info.AddValue("_name", _name);
        base.OnGetState(info, mode);
      }

      protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, Csla.Core.StateMode mode)
      {
        _name = info.GetValue<string>("_name");
        base.OnSetState(info, mode);
      }
    }
  }
}