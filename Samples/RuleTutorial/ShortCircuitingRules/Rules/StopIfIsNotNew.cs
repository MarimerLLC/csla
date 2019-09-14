// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopIfIsNotNew.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   Shorcircuits rule processing if object is not new.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using Csla.Rules;

namespace ShortCircuitingRules.Rules
{
  using Csla.Core;

  /// <summary>
  /// Shorcircuits rule processing if object is not new.
  /// </summary>
  public class StopIfIsNotNew : PropertyRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="StopIfIsNotNew"/> class.
    /// </summary>
    /// <param name="primaryProperty">
    /// The primary property.
    /// </param>
    public StopIfIsNotNew(IPropertyInfo primaryProperty)
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
      var target = (Csla.Core.ITrackStatus)context.Target;
      if (!target.IsNew)
      {
        context.AddSuccessResult(true);
      }
    }
  }
}
