// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollapseSpace.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   Removes leading, trailing and duplicate spaces.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Csla.Core;
using Csla.Rules;

namespace TransformationRules.Rules
{
  /// <summary>
  /// Removes leading, trailing and duplicate spaces.
  /// </summary>
  public class CollapseSpace : PropertyRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CollapseSpace"/> class.
    /// </summary>
    /// <param name="primaryProperty">
    /// The primary property.
    /// </param>
    public CollapseSpace(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties = new List<IPropertyInfo> { primaryProperty };
    }

    /// <summary>
    /// Business or validation rule implementation.
    /// </summary>
    /// <param name="context">
    /// Rule context object.
    /// </param>
    protected override void Execute(IRuleContext context)
    {
      var value = (string)context.InputPropertyValues[PrimaryProperty];
      if (string.IsNullOrEmpty(value)) return;

      var newValue = value.Trim(' ');
      var r = new Regex(@" +");
      newValue = r.Replace(newValue, @" ");
      context.AddOutValue(newValue);

      if (context.IsCheckRulesContext)
        Console.WriteLine(".... Rule {0} running from CheckRules", this.GetType().Name);
      else
        Console.WriteLine(".... Rule {0} running from {1} was changed", this.GetType().Name, this.PrimaryProperty.Name);
    }
  }
}
