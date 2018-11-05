// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalcSum.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   CalcSum rule will set primary property to the sum of all supplied properties.
//   Rule should run on client when a property is changed or when CheckRules is called.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Csla.Core;
using Csla.Rules;

namespace TransformationRules.Rules
{

  /// <summary>
  /// CalcSum rule will set primary property to the sum of all supplied properties.
  /// 
  /// Rule should run on client when a property is changed or when CheckRules is called.
  /// </summary>
  /// <remarks>
  /// As InputProperties is now regarded as Dependency you will not need to add a Dependency  
  /// rule to each input field in order to rerun calculation whenever on of the inputs is changed.
  /// </remarks>
  public class CalcSum : PropertyRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CalcSum"/> class.
    /// </summary>
    /// <param name="primaryProperty">
    /// The primary property.
    /// </param>
    /// <param name="inputProperties">
    /// The input properties.
    /// </param>
    public CalcSum(IPropertyInfo primaryProperty, params IPropertyInfo[] inputProperties)
      : base(primaryProperty)
    {
      InputProperties = new List<IPropertyInfo>();
      InputProperties.AddRange(inputProperties);
      this.RuleUri.AddQueryParameter("input", string.Join(",", inputProperties.Select(p => p.Name).ToArray()));
      CanRunOnServer = false;
    }

    /// <summary>
    /// Business or validation rule implementation.
    /// </summary>
    /// <param name="context">
    /// Rule context object.
    /// </param>
    protected override void Execute(IRuleContext context)
    {
      // Use linq Sum to calculate the sum value
      var sum = context.InputPropertyValues.Sum(property => (dynamic)property.Value);

      // add calculated value to OutValues
      // When rule is completed the RuleEngine will update businessobject
      context.AddOutValue(PrimaryProperty, sum);

      if (context.IsCascadeContext)
         Console.WriteLine(".... Rule {0} running from affected property or input property", this.GetType().Name);
      else if (context.IsCheckRulesContext)
        Console.WriteLine(".... Rule {0} running from CheckRules", this.GetType().Name);
      else
        Console.WriteLine(".... Rule {0} running from {2} was changed", this.GetType().Name, this.PrimaryProperty.Name);
    }
  }
}
