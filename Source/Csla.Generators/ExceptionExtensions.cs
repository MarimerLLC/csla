//-----------------------------------------------------------------------
// <copyright file="ExceptionExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extension methods for the Exception type</summary>
//-----------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Generators
{

  /// <summary>
  /// Extension methods for the Exception type
  /// </summary>
  internal static class ExceptionExtensions
  {

    /// <summary>
    /// Generate a Roslyn diagnostic from an exception 
    /// Can be used for reporting an issue during compilation
    /// </summary>
    /// <param name="ex">The raised exception that must be converted</param>
    /// <returns>A diagnostic suitable for exposing to the Rosyln compiler</returns>
    internal static Diagnostic ToUsageDiagnostic(this Exception ex)
    {
      Diagnostic diagnostic;
      DiagnosticDescriptor descriptor;

      descriptor = new DiagnosticDescriptor("Csla0001",
        ex.Message, GenerateSingleLineExceptionMessage(ex), "Usage", DiagnosticSeverity.Error, true,
        customTags: new string[] { WellKnownDiagnosticTags.Compiler, WellKnownDiagnosticTags.NotConfigurable });

      diagnostic = Diagnostic.Create(descriptor, null);

      return diagnostic;
    }

    #region Private Helper Methods

    /// <summary>
    /// Generate a single-line detailed error message from an exception, for use in diagnostics
    /// </summary>
    /// <param name="ex">The exception whose full message we are to report</param>
    /// <returns>A string containing the full details of an exception using semicolon separators</returns>
    private static string GenerateSingleLineExceptionMessage(Exception ex)
    {
      return ex.ToString().Replace(Environment.NewLine, "; ");
    }

    #endregion

  }
}
