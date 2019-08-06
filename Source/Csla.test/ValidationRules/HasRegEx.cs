//-----------------------------------------------------------------------
// <copyright file="HasRegEx.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.ValidationRules
{
  public class HasRegEx : BusinessBase<HasRegEx>
  {
    public static PropertyInfo<string> SsnProperty = RegisterProperty<string>(c => c.Ssn);
    public string Ssn
    {
      get { return GetProperty(SsnProperty); }
      set { SetProperty(SsnProperty, value); }
    }

    public static PropertyInfo<string> Ssn2Property = RegisterProperty<string>(c => c.Ssn2);
    public string Ssn2
    {
      get { return GetProperty(Ssn2Property); }
      set { SetProperty(Ssn2Property, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.RegExMatch(SsnProperty, @"^\d{3}-\d{2}-\d{4}$"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.RegExMatch(Ssn2Property, @"^\d{3}-\d{2}-\d{4}$"));
    }
  }
}