//-----------------------------------------------------------------------
// <copyright file="ExtractedOperationMethod.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Represents a single data portal operation method</summary>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Csla.Generator.DataPortalInterfaces.CSharp.Discovery;

namespace Csla.Generator.DataPortalInterfaces.CSharp.Extractors
{

  /// <summary>
  /// Represents a single data portal operation method extracted
  /// from the target type
  /// </summary>
  public class ExtractedOperationMethod
  {

    /// <summary>
    /// The name of the method
    /// </summary>
    public string MethodName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the method returns Task (async)
    /// </summary>
    public bool IsAsync { get; set; }

    /// <summary>
    /// The fully qualified attribute type names on this method
    /// (e.g. "Csla.CreateAttribute", "Csla.FetchAttribute")
    /// </summary>
    public IList<string> OperationAttributeNames { get; } = new List<string>();

    /// <summary>
    /// The criteria (non-injected) parameters
    /// </summary>
    public IList<ExtractedOperationParameter> CriteriaParameters { get; } = new List<ExtractedOperationParameter>();

    /// <summary>
    /// The injected parameters (marked with [Inject])
    /// </summary>
    public IList<ExtractedOperationParameter> InjectParameters { get; } = new List<ExtractedOperationParameter>();

    /// <summary>
    /// Computes the deterministic operation name for a given attribute.
    /// Format: "OperationType" for no criteria, "OperationType__Type1_Type2" for criteria.
    /// </summary>
    public string GetOperationName(string attributeFullName)
    {
      var baseName = OperationMethodExtractor.GetBaseOperationName(attributeFullName);
      if (CriteriaParameters.Count == 0)
        return baseName;
      var typeKeys = string.Join("_", CriteriaParameters.Select(p => p.TypeMetadataName));
      return baseName + "__" + typeKeys;
    }
  }
}
