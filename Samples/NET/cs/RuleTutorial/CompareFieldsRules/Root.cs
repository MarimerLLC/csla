// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The root.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;

using CompareFieldsRules.Rules;

using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

using GreaterThan = CompareFieldsRules.Rules.GreaterThan;
using GreaterThanOrEqual = CompareFieldsRules.Rules.GreaterThanOrEqual;
using LessThan = CompareFieldsRules.Rules.LessThan;

namespace CompareFieldsRules
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
    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(c => c.Num1, null, 8);

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
    /// The start date property.
    /// </summary>
    public static readonly PropertyInfo<SmartDate> StartDateProperty = RegisterProperty<SmartDate>(c => c.StartDate, null, new SmartDate() {Date = new Func<DateTime>(() => DateTime.Now).Invoke()});

    /// <summary>
    /// Gets or sets StartDate.
    /// </summary>
    public string StartDate
    {
      get { return GetPropertyConvert<SmartDate, string>(StartDateProperty); }
      set { SetPropertyConvert<SmartDate, string>(StartDateProperty, value); }
    }

    /// <summary>
    /// The end date property.
    /// </summary>
    public static readonly PropertyInfo<SmartDate> EndDateProperty = RegisterProperty<SmartDate>(c => c.EndDate, null, new SmartDate());

    /// <summary>
    /// Gets or sets EndDate.
    /// </summary>
    public string EndDate
    {
      get { return GetPropertyConvert<SmartDate, string>(EndDateProperty); }
      set { SetPropertyConvert<SmartDate, string>(EndDateProperty, value); }
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

      // add authorization rules 
      BusinessRules.AddRule(new GreaterThanOrEqual(Num2Property, Num1Property));
      BusinessRules.AddRule(new LessThan(StartDateProperty, EndDateProperty));
      BusinessRules.AddRule(new GreaterThan(EndDateProperty, StartDateProperty));
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

  }
}
