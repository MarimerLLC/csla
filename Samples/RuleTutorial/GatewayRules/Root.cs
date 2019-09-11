// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The root.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;

using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

using GatewayRules.Rules;

namespace GatewayRules
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
    /// The add business rules.
    /// </summary>
    protected override void AddBusinessRules()
    {
      // call base class implementation to add data annotation rules to BusinessRules 
      base.AddBusinessRules();

      // add authorization rules 
      BusinessRules.AddRule(new IsInRole(AuthorizationActions.WriteProperty, NoteProperty, "nobody"));

      // required only when user can write to field
      BusinessRules.AddRule(new CanWrite(NoteProperty, new Required(NoteProperty)));

      // different rules for Name whether object is New or Existing. 
      BusinessRules.AddRule(new IsNew(NameProperty, new Required(NameProperty)));
      BusinessRules.AddRule(new IsNotNew(NameProperty, new MaxLength(NameProperty, 50)));

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
      using (BypassPropertyChecks)
      {
        Name = "Rocky Lhotka";
        Num1 = 1001;
        Num2 = 666;
      }

      base.DataPortal_Create();
    }

    #endregion
  }
}
