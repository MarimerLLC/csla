using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Core;
using Csla.Rules;

namespace AddonPropertyRules.Rules
{
  /// <summary> 
  /// Rule class for RaisePropertyChanged on dependent properties
  /// </summary>
  public class RaisePropertyChanged : Csla.Rules.BusinessRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RaisePropertyChanged" /> class.
    /// </summary>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="raiseChangedProperties">The raise changed properties.</param>
    public RaisePropertyChanged(IPropertyInfo primaryProperty, params IPropertyInfo[] raiseChangedProperties)
      : base(primaryProperty)
    {
      AffectedProperties.AddRange(raiseChangedProperties);
    }
  }
}
