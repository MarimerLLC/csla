﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The root.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

using CustomAuthzRules.Rules;

using StopIfNotCanWrite = CustomAuthzRules.Rules.StopIfNotCanWrite;

namespace AuthzReadWriteProperty
{
  /// <summary>
  /// The root.
  /// </summary>
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    #region Business Methods

    /// <summary>
    /// The name property.
    /// </summary>
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    /// <summary>
    /// The num 1 property.
    /// </summary>
    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(c => c.Num1, "Num1", 9);

    /// <summary>
    /// Gets or sets Num1.
    /// </summary>
    public int Num1
    {
      get { return GetProperty(Num1Property); }
      set { SetProperty(Num1Property, value); }
    }

    /// <summary>
    /// The num 2 property.
    /// </summary>
    public static readonly PropertyInfo<int> Num2Property = RegisterProperty<int>(c => c.Num2);

    /// <summary>
    /// Gets or sets Num2.
    /// </summary>
    public int Num2
    {
      get { return GetProperty(Num2Property); }
      set { SetProperty(Num2Property, value); }
    }

    /// <summary>
    /// The country property.
    /// </summary>
    public static readonly PropertyInfo<string> CountryProperty = RegisterProperty<string>(c => c.Country);

    /// <summary>
    /// Gets or sets Country.
    /// </summary>
    public string Country
    {
      get { return GetProperty(CountryProperty); }
      set { SetProperty(CountryProperty, value); }
    }

    /// <summary>
    /// The state property.
    /// </summary>
    public static readonly PropertyInfo<string> StateProperty = RegisterProperty<string>(c => c.State);

    /// <summary>
    /// Gets or sets State.
    /// </summary>
    [Required]
    public string State
    {
      get { return GetProperty(StateProperty); }
      set { SetProperty(StateProperty, value); }
    }
    #endregion

    #region Validation Rules

    /// <summary>
    /// The add business rules.
    /// </summary>
    protected override void AddBusinessRules()
    {
      // call base class implementation to add data annotation rules to BusinessRules 
      base.AddBusinessRules();

      // State property onlyu writable when Country == "US"
      BusinessRules.AddRule(new OnlyForUS(AuthorizationActions.WriteProperty, StateProperty, CountryProperty));

      // Short circuit rule prosessing for State property when user is not allowed to edit field
      BusinessRules.AddRule(new StopIfNotCanWrite(StateProperty) {Priority = -1});

      BusinessRules.AddRule(new Dependency(CountryProperty, StateProperty));
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// The new editable root.
    /// </summary>
    /// <returns>
    /// </returns>
    public static Root NewEditableRoot()
    {
      return DataPortal.Create<Root>();
    }


    #endregion

    #region Data Access

    /// <summary>
    /// The data portal_ create.
    /// </summary>
    protected override void DataPortal_Create()
    {
      base.DataPortal_Create();
    }

    #endregion
  }
}
