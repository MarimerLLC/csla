//-----------------------------------------------------------------------
// <copyright file="CslaBaseClasses.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The CSLA base classes whose properties can be generated</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoImplement
{
  /// <summary>
  /// The CSLA base classes whose properties can be generated, and the property
  /// methods the generated getter and setter call
  /// </summary>
  internal static class CslaBaseClasses
  {
    /// <summary>
    /// The property methods, by the name of the base class in the Csla namespace
    /// </summary>
    public static IReadOnlyDictionary<string, (string Getter, string Setter)> PropertyMethods { get; } =
      new Dictionary<string, (string Getter, string Setter)>
      {
        ["BusinessBase"] = ("GetProperty", "SetProperty"),
        ["BusinessDocumentBase"] = ("GetProperty", "SetProperty"),
        ["ReadOnlyBase"] = ("GetProperty", "LoadProperty"),
        ["CommandBase"] = ("ReadProperty", "LoadProperty")
      };
  }
}
