//-----------------------------------------------------------------------
// <copyright file="TrackingNames.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tracking names for validating incremental generator caching</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoImplementProperties.CSharp.MobileSerializers
{
  /// <summary>
  /// Names of incremental pipeline steps, used by tests to verify caching.
  /// </summary>
  public static class TrackingNames
  {
    /// <summary>Extraction of registrations from <c>[MobileSerializer]</c>.</summary>
    public const string ExtractTypeSideRegistrations = nameof(ExtractTypeSideRegistrations);

    /// <summary>Extraction of registrations from <c>[MobileSerializerFor]</c>.</summary>
    public const string ExtractSerializerSideRegistrations = nameof(ExtractSerializerSideRegistrations);

    /// <summary>All registrations, merged and sorted.</summary>
    public const string MergeRegistrations = nameof(MergeRegistrations);
  }
}
