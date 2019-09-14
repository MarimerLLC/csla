// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLookupCustomer.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   An async lookup rule, typically used in rich clients to do (sync) data access
//   on a background thread and so keep the UI responsive.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;

using AsyncLookupRule.Commands;

using Csla.Core;
using Csla.Rules;

namespace AsyncLookupRule.Rules
{
  /// <summary>
  /// An async lookup rule, typically used in rich clients to do (sync) data access 
  /// on a background thread and so keep the UI responsive. 
  /// </summary>
  public class AsyncLookupCustomer : PropertyRule
  {
    /// <summary>
    /// Gets or sets NameProperty.
    /// </summary>
    private IPropertyInfo NameProperty { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLookupCustomer"/> class.
    /// </summary>
    /// <param name="primaryProperty">
    /// The primary property.
    /// </param>
    /// <param name="nameProperty">
    /// The name property.
    /// </param>
    public AsyncLookupCustomer(IPropertyInfo primaryProperty, IPropertyInfo nameProperty)
      : base(primaryProperty)
    {
      NameProperty = nameProperty;

      if (InputProperties == null)
        InputProperties = new List<IPropertyInfo>();
      InputProperties.Add(primaryProperty);
      

      AffectedProperties.Add(nameProperty);

      IsAsync = true;

      // setting all to false will only allow the rule to run when the property is set - typically by the user from the UI.
      CanRunAsAffectedProperty = false;
      CanRunInCheckRules = false;
      CanRunOnServer = false;
    }

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    protected override async void Execute(IRuleContext context)
    {
      var id = (int)context.InputPropertyValues[PrimaryProperty];

      try
      {
        var cmd = await LookupCustomerCommand.ExecuteAsync(id);
        context.AddOutValue(NameProperty, cmd.Name);
      }
      catch (Exception ex)
      {
        context.AddErrorResult(ex.Message);
      }
      context.Complete();
    }
  }
}
