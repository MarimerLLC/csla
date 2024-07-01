//-----------------------------------------------------------------------
// <copyright file="AddressPOCO.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class that can be used for testing serialization behaviour</summary>
//-----------------------------------------------------------------------
using Csla.Serialization;

namespace Csla.Generators.CSharp.TestObjects
{

  [AutoSerializable]
  public partial class AddressPOCO
  {

    public string? AddressLine1 { get; set; }

    public string AddressLine2 { get; set; } = string.Empty;

    public string Town { get; set; } = string.Empty;

    public string County { get; set; } = string.Empty;

    public string Postcode { get; set; } = string.Empty;

  }
}
