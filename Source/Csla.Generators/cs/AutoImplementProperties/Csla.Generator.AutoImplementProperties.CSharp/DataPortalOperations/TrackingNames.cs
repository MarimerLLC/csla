//-----------------------------------------------------------------------
// <copyright file="TrackingNames.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tracking names for validating incremental generator caching</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations
{
  /// <summary>
  /// Names of incremental pipeline steps, used by tests to verify caching.
  /// </summary>
  public static class TrackingNames
  {
    /// <summary>Extraction of individual operation methods.</summary>
    public const string ExtractOperationMethods = nameof(ExtractOperationMethods);

    /// <summary>Grouping of operation methods by containing type.</summary>
    public const string GroupOperationTypes = nameof(GroupOperationTypes);

    /// <summary>Per-type models for operations generation.</summary>
    public const string OperationTypes = nameof(OperationTypes);

    /// <summary>Extraction of types marked with DataPortalExtensions.</summary>
    public const string ExtractExtensionTypes = nameof(ExtractExtensionTypes);

    /// <summary>Per-type models for extension generation.</summary>
    public const string ExtensionTypes = nameof(ExtensionTypes);
  }
}
