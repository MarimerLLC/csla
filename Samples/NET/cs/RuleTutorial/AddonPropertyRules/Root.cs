// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The root.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using AddonPropertyRules.Rules;
using Csla;

namespace AddonPropertyRules
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

    public static readonly PropertyInfo<int> SumProperty = RegisterProperty<int>(c => c.Sum);
    public int Sum
    {
      get { return GetProperty(SumProperty); }
      set { SetProperty(SumProperty, value); }
    }

    /// <summary>
    /// The num 1 property.
    /// </summary>
    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(c => c.Num1, null, 0);

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

    public static readonly PropertyInfo<int> Num3Property = RegisterProperty<int>(c => c.Num3);
    public int Num3
    {
      get { return GetProperty(Num3Property); }
      set { SetProperty(Num3Property, value); }
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

    public static readonly PropertyInfo<string> String1Property = RegisterProperty<string>(c => c.String1);
    public string String1
    {
      get { return GetProperty(String1Property); }
      set { SetProperty(String1Property, value); }
    }

    public static readonly PropertyInfo<string> String2Property = RegisterProperty<string>(c => c.String2);
    public string String2
    {
      get { return GetProperty(String2Property); }
      set { SetProperty(String2Property, value); }
    }

    public static readonly PropertyInfo<string> ConcatStringProperty = RegisterProperty<string>(c => c.ConcatString);
    public string ConcatString
    {
      get { return string.Format("{0} {1}", ReadProperty(String1Property), ReadProperty(String2Property)); }
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
      BusinessRules.AddRule(new DependencyFrom(SumProperty, Num1Property, Num2Property, Num3Property));
      BusinessRules.AddRule(new Range(Num1Property, 1, 10, "Num1 must be between 1 and 10 to be valid."));
      BusinessRules.AddRule(new DependencyFrom(ConcatStringProperty, String1Property, String2Property));
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
