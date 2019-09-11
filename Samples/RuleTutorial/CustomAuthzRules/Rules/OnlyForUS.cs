// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlyForUS.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The only for us.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using Csla.Core;
using Csla.Rules;

namespace CustomAuthzRules.Rules
{
  /// <summary>
  /// The only for us.
  /// </summary>
  public class OnlyForUS : Csla.Rules.AuthorizationRule
  {
    /// <summary>
    /// Gets or sets CountryProperty.
    /// </summary>
    public IPropertyInfo CountryProperty { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlyForUS"/> class.
    /// </summary>
    /// <param name="action">
    /// The action.
    /// </param>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="countryProperty">
    /// The country property.
    /// </param>
    public OnlyForUS(AuthorizationActions action, IMemberInfo element, IPropertyInfo countryProperty)
      : base(action, element)
    {
      CountryProperty = countryProperty;
	  CacheResult = false;
    }

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    protected override void Execute(IAuthorizationContext context)
    {
      var value = (string) ReadProperty(context.Target, CountryProperty);

      context.HasPermission = value == "US";
    }
  }
}