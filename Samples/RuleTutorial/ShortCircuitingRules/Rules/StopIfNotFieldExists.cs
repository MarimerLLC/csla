// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopIfNotFieldExixts.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   Shorcircuits rule processing if lazy loaded field is not initalized.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using Csla.Rules;

namespace ShortCircuitingRules.Rules
{
  /// <summary>
  /// Shorcircuits rule processing if lazy loaded field is not initalized (ie: not included in InputPropertyValues).
  /// </summary>
  public class StopIfNotFieldExists : BusinessRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="StopIfNotFieldExists"/> class.
    /// </summary>
    /// <param name="primaryProperty">Primary property for this rule.</param>
    public StopIfNotFieldExists(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      if (InputProperties == null)
        InputProperties = new List<IPropertyInfo>();
      InputProperties.Add(primaryProperty);
    }

    /// <summary>
    /// Execut rule. 
    /// Short circuit if field is not included in InputPropertyValues.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    protected override void Execute(IRuleContext context)
    {

      if (!context.InputPropertyValues.ContainsKey(PrimaryProperty))
      {
        context.AddSuccessResult(true);
      }
    }
  }
}
