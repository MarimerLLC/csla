// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopIfNotCanWrite.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   Shorcircuits rule processing if user is not allowed to edit this field.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using Csla.Rules;

namespace ShortCircuitingRules.Rules
{
  using Csla.Core;

  /// <summary>
  /// Shorcircuits rule processing if user is not allowed to edit this field.
  /// </summary>
  public class StopIfNotCanWrite : BusinessRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="StopIfNotCanWrite"/> class.
    /// </summary>
    /// <param name="primaryProperty">
    /// The primary property.
    /// </param>
    public StopIfNotCanWrite(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    { }

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    protected override void Execute(IRuleContext context)
    {
      var target = (Csla.Core.BusinessBase)context.Target;

      // Short circuit rule processing for this property if user is not allowed to edit field.
      if (!target.CanWriteProperty(PrimaryProperty))
      {
        context.AddSuccessResult(true);
      }
    }
  }
}
