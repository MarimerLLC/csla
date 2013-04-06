using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Validation
{
  /// <summary>
  /// Delegate defining a handler for an async validation rule
  /// result.
  /// </summary>
  /// <param name="result">
  /// Result arguments from a validation rule method.
  /// </param>
  public delegate void AsyncRuleResultHandler(AsyncRuleResult result);
}
