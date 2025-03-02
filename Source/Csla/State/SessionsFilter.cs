//-----------------------------------------------------------------------
// <copyright file="SessionsFilter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Filter for querying session storage</summary>
//-----------------------------------------------------------------------
#nullable enable

namespace Csla.State
{
  /// <summary>
  /// Filter to query sessions
  /// </summary>
  public class SessionsFilter
  {
    /// <summary>
    /// A timespan to filter sessions last touched after the expiration
    /// </summary>
    public TimeSpan? Expiration { get; set; }

    /// <summary>
    /// Validates
    /// </summary>
    public void Validate()
    {
      if (!Expiration.HasValue)
      {
        throw new ArgumentNullException("Expiration is required.");
      }
    }
  }
}
