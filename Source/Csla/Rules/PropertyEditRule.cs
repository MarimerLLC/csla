using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// 
  /// </summary>
  public abstract class PropertyEditRule : PropertyRule
  {
    /// <summary>
    /// Base class for a Property Edited rule.
    /// This rule will only execute on the client when the property is 
    /// edited, is an affected property of another rule, 
    /// when one of the input properties is changed or CheckRules is called.
    /// These rules will NOT execute on the server. side of the DataPortal.
    /// </summary>
    /// <param name="propertyInfo">The property info.</param>
    protected PropertyEditRule(IPropertyInfo propertyInfo)
      : base(propertyInfo)
    {
      CanRunAsAffectedProperty = true;
      CanRunInCheckRules = true;
      CanRunOnServer = false;

    }
  }
}
