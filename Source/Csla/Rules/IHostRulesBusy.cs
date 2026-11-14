//-----------------------------------------------------------------------
// <copyright file="IHostRulesBusy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Object-level busy notifications from the rule engine</summary>
//-----------------------------------------------------------------------

namespace Csla.Rules
{
  /// <summary>
  /// Receives object-level busy notifications from the rule engine.
  /// </summary>
  /// <remarks>
  /// A rule stream that waits for a sequential async rule with no
  /// affected properties keeps the object busy with no busy property, so
  /// there is no <see cref="IHostRules.PropertyIdle"/> call to report the
  /// object idle. The rule engine reports it through this interface.
  /// </remarks>
  internal interface IHostRulesBusy
  {
    /// <summary>
    /// Called when the object becomes busy or idle with no busy property.
    /// </summary>
    /// <param name="busy">True when the object becomes busy.</param>
    void ObjectBusyChanged(bool busy);
  }
}
