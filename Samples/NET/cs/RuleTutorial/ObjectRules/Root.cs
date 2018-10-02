// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The root.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;

using Csla;
using Csla.Core;
using Csla.Rules;

namespace ObjectRules
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
    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(c => c.Num1);

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

    #endregion

    #region Validation Rules


    /// <summary>
    /// Override the PropertyHasChanged method to add CheckObject rules '
    /// whenever one of the properties have been changed. 
    /// </summary>
    /// <param name="property">
    /// The property.
    /// </param>
    protected override void PropertyHasChanged(IPropertyInfo property)
    {
      base.PropertyHasChanged(property);

      // BusinessRules.CheckObjectRules();
    }

    /// <summary>
    /// The check all rules.
    /// </summary>
    public void CheckAllRules()
    {
      CheckObjectRules();
    }

    /// <summary>
    /// The add business rules.
    /// </summary>
    protected override void AddBusinessRules()
    {
      // call base class implementation to add data annotation rules to BusinessRules 
      base.AddBusinessRules();

      // get all registered properties 
      var props = this.FieldManager.GetRegisteredProperties();

      // add authorization rules - send all properties to the ObjectRule so they are availble for AddZYZResult 
      BusinessRules.AddRule(new ValidateRoot(props));
    }

    /// <summary>
    /// ObjectRules may be used to integrate with an external rule engine. 
    /// </summary>
    public class ValidateRoot : Csla.Rules.ObjectRule
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="ValidateRoot"/> class.
      /// </summary>
      /// <param name="fields">
      /// The fields.
      /// </param>
      public ValidateRoot(IEnumerable<IPropertyInfo> fields)
      {
        AffectedProperties.AddRange(fields);
      }

      /// <summary>
      /// The execute.
      /// </summary>
      /// <param name="context">
      /// The context.
      /// </param>
      protected override void Execute(IRuleContext context)
      {
        var bo = (Csla.Core.BusinessBase) context.Target;
        var name = (string) ReadProperty(context.Target, Root.NameProperty);

        if (string.IsNullOrEmpty(name))
          context.AddErrorResult(NameProperty, "Name is required.");
        else if (name.Length > 50)
          context.AddErrorResult(NameProperty, "Name cannot be longer then 50 chars.");

        var num1 = (int) ReadProperty(context.Target, Root.Num1Property);
        if (num1 < 5)
          context.AddErrorResult(Root.Num1Property, "Num1 must be larger than or equal to 5");
      }
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
