using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// Base class for object level rules.
  /// The property PrimaryProperty must always return null
  /// </summary>
  public abstract class ObjectRule : BusinessRule
  {
    /// <summary>
    /// Gets or sets the severity for this rule.
    /// </summary>
    public RuleSeverity Severity { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectRule"/> class.
    /// </summary>
    protected ObjectRule()
    {
      Severity = RuleSeverity.Error;
    }

    /// <summary>
    /// Gets or sets the primary property affected by this rule.
    /// </summary>
    public override IPropertyInfo PrimaryProperty
    {
      get
      {
        return null;
      }
      set
      {
        if (value != null)
        {
          throw new ArgumentException("Object rule can not have PrimaryPropery.");
        }
      }
    }
  }
}
