using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Validation
{
  /// <summary>
  /// Delegate for handling the completion of an
  /// async validation rule.
  /// </summary>
  /// <param name="sender">
  /// Object calling the handler.
  /// </param>
  /// <param name="result">
  /// Result arguments from the validation rule.
  /// </param>
  public delegate void AsyncRuleCompleteHandler(object sender, AsyncRuleResult result);
}
