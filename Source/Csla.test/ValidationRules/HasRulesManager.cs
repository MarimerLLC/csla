//-----------------------------------------------------------------------
// <copyright file="HasRulesManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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
  public partial class HasRulesManager : BusinessBase<HasRulesManager>
  {
    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static void NewHasRulesManager(EventHandler<DataPortalResult<HasRulesManager>> completed)
    {
      Csla.DataPortal.BeginCreate<HasRulesManager>(
        new Criteria(),
        completed);
    }

    public static void GetHasRulesManager(string name, EventHandler<DataPortalResult<HasRulesManager>> completed)
    {
      Csla.DataPortal.BeginFetch<HasRulesManager>(
        new Criteria(name),
        completed);
    }

    public static void DeleteHasRulesManager(string name)
    {
      Csla.DataPortal.BeginDelete<HasRulesManager>(
        new Criteria(name),
        (o, e) => { });
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(NameProperty, 10));
    }

    [Serializable()]
    private class Criteria : CriteriaBase<Criteria>
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
        base.OnGetState(info, mode);
        info.AddValue("_name", _name);
      }

      protected override void OnSetState(SerializationInfo info, StateMode mode)
      {
        base.OnSetState(info, mode);
        _name = info.GetValue<string>("_name");
      }
    }
  }
}