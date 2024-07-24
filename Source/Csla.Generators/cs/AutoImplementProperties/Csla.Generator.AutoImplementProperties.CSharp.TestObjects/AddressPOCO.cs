//-----------------------------------------------------------------------
// <copyright file="AddressPOCO.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class that can be used for testing serialization behaviour</summary>
//-----------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;
using Csla.Serialization;

namespace Csla.Generator.AutoImplementProperties.CSharp.TestObjects
{
  [CslaImplementProperties]
  public partial class AddressPOCO : BusinessBase<AddressPOCO>
  {
    [Display(Name = "Address Line 1")]
    public partial string? AddressLine1 { get; private set; }

    public partial string AddressLine2 { get; set; }

    public partial string Town { get; set; }

    public partial string County { get; set; }

    public partial string Postcode { get; set; }

    [CslaIgnoreProperty]
    public partial string IgnoredProperty { get; set; }

  }
}
