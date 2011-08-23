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
        InputProperties = new List<IPropertyInfo>() { PrimaryProperty };
      AffectedProperties.Add(nameProperty);

      IsAsync = false;
      CanRunAsAffectedProperty = false;
      CanRunInCheckRules = true;
      CanRunOnServer = false;
    }

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    protected override void Execute(RuleContext context)
    {
      var id = (int) context.InputPropertyValues[PrimaryProperty];

      // use a command or read-only object for lookup
      var lookup = LookupCustomerCommand.Execute(id);

      context.AddOutValue(NameProperty, lookup.Name);
    }
  }
}
