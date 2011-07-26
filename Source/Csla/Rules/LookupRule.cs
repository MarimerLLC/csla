using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// Base class for an Async Lookup rule (by default has IsAsync = true) 
  /// that will only run on the client side of DataPortal when a property 
  /// has been edited by a user.
  /// </summary>
  public abstract  class LookupRule : PropertyRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LookupRule"/> class.
    /// </summary>
    /// <param name="primaryProperty">The primary property.</param>
    protected LookupRule(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      IsAsync = true;
      CanRunOnServer = false;
      CanRunInCheckRules = false;
      CanRunAsAffectedProperty = false;
    }
  }
}