using Csla.Core;
using Csla.Rules;

namespace Templates
{
  public class AuthorizationRuleClass : AuthorizationRule
  {
    // TODO: Add additional parameters to your rule to the constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationRuleClass"/> class.
    /// </summary>
    /// <param name="action">Action this rule will enforce.</param>
    /// <param name="element">Method or property.</param>
    public AuthorizationRuleClass(AuthorizationActions action, IMemberInfo element)
      : base(action, element)
    {
      // TODO: Add additional constructor code here 

    }

    // TODO: Add additional parameters to your rule to the constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationRuleClass"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    public AuthorizationRuleClass(AuthorizationActions action)
      : base(action)
    {
      // TODO: Add additional constructor code here 

    }

    protected override void Execute(IAuthorizationContext context)
    {
      // TODO: Add actual rule code here. 
      //if (!access_condition)
      //{
      //  context.HasPermission = false;
      //}
    }
  }
}