using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using Csla.Rules;

namespace AddonPropertyRules.Rules
{
  /// <summary>
  /// A rule that establishes a dependency from properties to PrimaryProperty.
  /// The rules for PrimaryProperty will be rerun wheneve one of the depencyProperties is changed.
  /// NotifyPropertyChanged will also be raised for PrimaryProperty when any of input properties is changed. 
  /// </summary>
  public class DependencyFrom : BusinessRule
  {
    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Primary property for the rule.</param>
    /// <param name="dependencyProperties">Dependent property.</param>
    /// <remarks>
    /// When rules are run for one of the dependency properties the rules for primary property i rerun.
    /// </remarks>
    public DependencyFrom(Csla.Core.IPropertyInfo primaryProperty, params Csla.Core.IPropertyInfo[] dependencyProperties)
      : base(primaryProperty)
    {
      if (InputProperties == null) 
        InputProperties = new List<IPropertyInfo>();
      InputProperties.AddRange(dependencyProperties);
      RuleUri.AddQueryParameter("dependencyfrom", string.Join(",", dependencyProperties.Select(p => p.Name)));
    }

    /// <summary>
    /// Creates an instance of the rule.
    /// </summary>
    /// <param name="primaryProperty">Primary property for the rule.</param>
    /// <param name="dependencyProperty">The dependency property.</param>
    /// <param name="isBiDirectional">if set to <c>true</c> [is bi directional].</param>
    /// <remarks>
    /// When rules are run for one of the dependency properties the rules for primary property i rerun.
    /// </remarks>
    public DependencyFrom(Csla.Core.IPropertyInfo primaryProperty, Csla.Core.IPropertyInfo dependencyProperty,  bool isBiDirectional)
      : base(primaryProperty)
    {
      if (InputProperties == null)
        InputProperties = new List<IPropertyInfo>();
      InputProperties.Add(dependencyProperty);
      if (isBiDirectional) 
        AffectedProperties.Add(dependencyProperty);
    }
  }
}
