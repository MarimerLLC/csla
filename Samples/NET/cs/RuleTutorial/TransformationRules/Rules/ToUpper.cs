// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToUpper.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The to upper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;

using Csla.Core;
using Csla.Rules;

namespace TransformationRules.Rules
{
  /// <summary>
  /// The to upper.
  /// </summary>
  public class ToUpper : Csla.Rules.BusinessRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ToUpper"/> class.
    /// </summary>
    /// <param name="primaryProperty">
    /// The primary property.
    /// </param>
    public ToUpper(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties = new List<IPropertyInfo>(){primaryProperty};
      AffectedProperties.Add(primaryProperty);
    }

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    protected override void Execute(IRuleContext context)
    {
      var value = (string) context.InputPropertyValues[PrimaryProperty];
      context.AddOutValue(PrimaryProperty, value.ToUpper());

     if (context.IsCheckRulesContext)
        Console.WriteLine(".... Rule {0} running from CheckRules", this.GetType().Name);
      else
        Console.WriteLine(".... Rule {0} running from {1} was changed", this.GetType().Name, this.PrimaryProperty.Name);
    }
  }
}
