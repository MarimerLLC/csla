// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LookupCustomer.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   A typical LookupCustomer command for use in WEB applications
//   In a WEB app you will typically want to update all the fields as returned in the HTTP POST command
//   and call CheckRules to run all rules afterwards.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System.Collections.Generic;

using Csla.Core;
using Csla.Rules;

using LookupRule.Commands;

namespace LookupRule.Rules
{
  /// <summary>
  /// A typical LookupCustomer command for use in WEB applications
  /// 
  /// In a WEB app you will typically want to update all the fields as returned in the HTTP POST command
  /// and call CheckRules to run all rules afterwards. 
  /// </summary>
  public class LookupCustomer : PropertyRule
  {
    /// <summary>
    /// Gets or sets NameProperty.
    /// </summary>
    private IPropertyInfo NameProperty { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupCustomer"/> class.
    /// </summary>
    /// <param name="primaryProperty">
    /// The primary property.
    /// </param>
    /// <param name="nameProperty">
    /// The name property.
    /// </param>
    public LookupCustomer(IPropertyInfo primaryProperty, IPropertyInfo nameProperty)
      : base(primaryProperty)
    {
      NameProperty = nameProperty;

      if (InputProperties == null)
        InputProperties = new List<IPropertyInfo>();
      InputProperties.Add(primaryProperty);
      AffectedProperties.Add(nameProperty);

      IsAsync = false;
      CanRunAsAffectedProperty = false;   // only run when this property is changed 
      CanRunInCheckRules = true;          // when true will also run in CheckRules
      CanRunOnServer = false;             // when true will also run on logical serverside (Data Access)
    }

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    protected override void Execute(IRuleContext context)
    {
      var id = (int) context.InputPropertyValues[PrimaryProperty];

      // use a command or read-only object for lookup
      var lookup = LookupCustomerCommand.Execute(id);

      context.AddOutValue(NameProperty, lookup.Name);
    }
  }
}
