using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Validation
{
  /// <summary>
  /// Delegate defining an asynchronous validation rule method.
  /// </summary>
  /// <param name="context">
  /// Context parameters provided to the validation rule method.
  /// </param>
  public delegate void AsyncRuleHandler(AsyncValidationRuleContext context);
}
