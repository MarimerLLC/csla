// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LessThanOrEqual.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   Validates that primary property is less than or equal compareToProperty
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;

using CompareFieldsRules.Properties;

using Csla.Core;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace CompareFieldsRules.Rules
{
  /// <summary>
  /// Validates that primary property is less than or equal compareToProperty
  /// </summary>
  public class LessThanOrEqual : CommonBusinessRule
  {
    /// <summary>
    /// Gets or sets CompareTo.
    /// </summary>
    private IPropertyInfo CompareTo { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompareFieldsRules.Rules.LessThanOrEqual"/> class.
    /// </summary>
    /// <param name="primaryProperty">
    /// The primary property.
    /// </param>
    /// <param name="compareToProperty">
    /// The compare to property.
    /// </param>
    public LessThanOrEqual(IPropertyInfo primaryProperty, IPropertyInfo compareToProperty)
      : base(primaryProperty)
    {
      CompareTo = compareToProperty;
      InputProperties = new List<IPropertyInfo>() { primaryProperty, compareToProperty };
      this.RuleUri.AddQueryParameter("compareto", compareToProperty.Name);
    }

    /// <summary>
    /// Gets the message.
    /// </summary>
    /// <returns>
    /// The get message.
    /// </returns>
    protected override string GetMessage()
    {
      return HasMessageDelegate ? base.GetMessage() : Resources.LessThanOrEqualRule;
    }

    /// <summary>
    /// Does the check for primary propert less than compareTo property
    /// </summary>
    /// <param name="context">
    /// Rule context object.
    /// </param>
    protected override void Execute(IRuleContext context)
    {
      var value1 = (IComparable)context.InputPropertyValues[PrimaryProperty];
      var value2 = (IComparable)context.InputPropertyValues[CompareTo];

      if (value1.CompareTo(value2) > 0)
      {
        context.Results.Add(new RuleResult(this.RuleName, PrimaryProperty, string.Format(GetMessage(), PrimaryProperty.FriendlyName, CompareTo.FriendlyName)) { Severity = this.Severity });
      }
    }
  }

}
