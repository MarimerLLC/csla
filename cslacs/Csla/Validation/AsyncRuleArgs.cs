using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;

namespace Csla.Validation
{
  public class AsyncRuleArgs
  {
    public IPropertyInfo[] Properties { get; protected set; }

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
