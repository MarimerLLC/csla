//-----------------------------------------------------------------------
// <copyright file="ExtractedOperationParameter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Represents a parameter of a data portal operation method</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.DataPortalInterfaces.CSharp.Extractors
{

  /// <summary>
  /// Represents a parameter of a data portal operation method
  /// </summary>
  public class ExtractedOperationParameter
  {

    /// <summary>
    /// The parameter name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The fully qualified type name of the parameter
    /// </summary>
    public string TypeFullName { get; set; } = string.Empty;

    /// <summary>
    /// The type name suitable for use in generated code
    /// (may include global:: prefix)
    /// </summary>
    public string TypeDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The type's metadata name used for operation name computation
    /// (e.g. "Int32", "String", "List_1_Int32")
    /// </summary>
    public string TypeMetadataName { get; set; } = string.Empty;

    /// <summary>
    /// Whether this is an injected parameter
    /// </summary>
    public bool IsInjected { get; set; }

    /// <summary>
    /// For injected parameters, whether null is allowed
    /// (maps to [Inject(AllowNull = true)])
    /// </summary>
    public bool AllowNull { get; set; }
  }
}
