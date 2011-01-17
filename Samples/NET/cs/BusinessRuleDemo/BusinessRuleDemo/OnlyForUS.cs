using Csla.Core;
using Csla.Reflection;
using Csla.Rules;

namespace BusinessRuleDemo
{
  public class OnlyForUS : Csla.Rules.AuthorizationRule
  {

    private IMemberInfo CountryField { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="OnlyForUS"/> class.
    /// </summary>
    /// <param name="action">Action this rule will enforce.</param>
    /// <param name="element">Method or property.</param>
    public OnlyForUS(AuthorizationActions action, IMemberInfo element, IMemberInfo countryField)
      : base(action, element)
    {
      CountryField = countryField;
    }

    /// <summary>
    /// Gets a value indicating whether [cache result].
    /// </summary>
    /// <value><c>true</c> if [cache result]; otherwise, <c>false</c>.</value>
    public override bool CacheResult
    {
      get
      {
        return false;
      }
    }

    protected override void Execute(AuthorizationContext context)
    {
      var country = (string)MethodCaller.CallPropertyGetter(context.Target, CountryField.Name);
      context.HasPermission = country.Equals(CountryNVL.UnitedStates);
    }
  }
}