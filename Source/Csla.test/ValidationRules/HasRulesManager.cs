//-----------------------------------------------------------------------
// <copyright file="HasRulesManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Serialization.Mobile;

namespace Csla.Test.ValidationRules
{
  [Serializable]
  public partial class HasRulesManager : BusinessBase<HasRulesManager>
  {
    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Rules.CommonRules.Required(NameProperty));
      BusinessRules.AddRule(new Rules.CommonRules.MaxLength(NameProperty, 10));
    }

    [Serializable]
    public class Criteria : CriteriaBase<Criteria>
    {
      public string _name;

      public Criteria()
      {
        _name = "<new>";
      }

      public Criteria(string name)
      {
        _name = name;
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