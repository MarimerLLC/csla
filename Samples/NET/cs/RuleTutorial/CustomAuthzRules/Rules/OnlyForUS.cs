using Csla.Core;
using Csla.Rules;

namespace CustomAuthzRules.Rules
{
  public class OnlyForUS : Csla.Rules.AuthorizationRule
  {
    public IPropertyInfo CountryProperty { get; set; }

    public OnlyForUS(AuthorizationActions action, IMemberInfo element, IPropertyInfo countryProperty)
      : base(action, element)
    {
      CountryProperty = countryProperty;
    }

    public override bool CacheResult
    {
      get
      {
        return false; 
      }
    }


    protected override void Execute(AuthorizationContext context)
    {
      var value = (string) ReadProperty(context.Target, CountryProperty);

      context.HasPermission = value == "US";
    }
  }
}