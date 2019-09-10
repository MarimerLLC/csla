// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The root.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.ComponentModel.DataAnnotations;

using Csla;
using Csla.Rules.CommonRules;

using ShortCircuitingRules.Rules;

using StopIfIsNotNew = ShortCircuitingRules.Rules.StopIfIsNotNew;

namespace ShortCircuitingRules
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
    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    /// <summary>
    /// The num 1 property.
    /// </summary>
    [Range(0, 50)] 
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

    /// <summary>
    /// The note property.
    /// </summary>
    public static readonly PropertyInfo<string> NoteProperty = RegisterProperty<string>(c => c.Note);

    /// <summary>
    /// Gets or sets Note.
    /// </summary>
    public string Note
    {
      get { return GetProperty(NoteProperty); }
      set { SetProperty(NoteProperty, value); }
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
      // Required as DataAnnotationRuleon Name property
      // add maxlength with higher priority so it will not execute if Required is already broken.
      // NOTE: BusinessRules.ProcessThroughPriority is default 0 meaning that all rules at Priority lower or equal to 0 is always run, unless StopProcessing is called explicitly.
      BusinessRules.AddRule(new MaxLength(NameProperty, 50) { Priority = 1 });  
      BusinessRules.AddRule(new MinValue<int>(Num2Property, 5));
      BusinessRules.AddRule(new Required(NoteProperty));


      // ShortCircuit (ie do not run rules) for these properties when object has IsNew = false
      // DataAnnotation rules is always added with priority -1 so by giving ShortCircuiting rules a priority of -1 
      // you can also block DataAnootation rules from being executed. 
      // The same may also be done with StopINotCanWrite to prevent validation of fields that the user is not allowed to edit. 
      BusinessRules.AddRule(new StopIfIsNotNew(NameProperty) {Priority = -1});
      BusinessRules.AddRule(new StopIfIsNotNew(Num2Property) {Priority = -1});
      BusinessRules.AddRule(new StopIfIsNotNew(Num1Property) {Priority = -1});

      // only validates Note when user is allowed to edit field.
      BusinessRules.AddRule(new StopIfNotCanWrite(NoteProperty) {Priority = -1});

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

    /// <summary>
    /// The get editable root.
    /// </summary>
    /// <returns>
    /// </returns>
    public static Root GetEditableRoot()
    {
      return DataPortal.Fetch<Root>(null);
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

    /// <summary>
    /// The data portal_ fetch.
    /// </summary>
    /// <param name="criteria">
    /// The criteria.
    /// </param>
    protected void DataPortal_Fetch(object criteria)
    {
      BusinessRules.CheckRules();
    }

    #endregion
  }
}
