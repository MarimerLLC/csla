using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;

namespace Csla.Validation
{
  /// <summary>
  /// Arguments provided to an async 
  /// validation rule method.
  /// </summary>
  public class AsyncRuleArgs
  {
    /// <summary>
    /// List of the property values to be made available
    /// to this validation rule.
    /// </summary>
    public IPropertyInfo[] Properties { get; protected set; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="primaryProperty">
    /// The primary property to be validated by this
    /// rule method.
    /// </param>
    /// <param name="additionalProperties">
    /// A list of additional property values to be
    /// provided to the validationr rule.
    /// </param>
    public AsyncRuleArgs(IPropertyInfo primaryProperty, params IPropertyInfo[] additionalProperties)
    {
      if (primaryProperty == null)
        throw new ArgumentNullException("primaryProperty");

      int length = 1;
      if(additionalProperties != null)
        length += additionalProperties.Length;

      Properties = new IPropertyInfo[length];
      Properties[0] = primaryProperty;

      if (additionalProperties != null)
        Array.Copy(additionalProperties, 0, Properties, 1, additionalProperties.Length);
    }
  }
}
