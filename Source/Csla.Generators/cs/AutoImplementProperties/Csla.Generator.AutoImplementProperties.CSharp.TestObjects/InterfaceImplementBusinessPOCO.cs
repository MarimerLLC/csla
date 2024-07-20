﻿using System.ComponentModel.DataAnnotations;
using Csla.Serialization;

namespace Csla.Generator.AutoImplementProperties.TestObjects
{
  [AutoImplementPropertiesInterface<IInterfaceImplementBusinessPOCO>]
  public partial class InterfaceImplementBusinessPOCO : BusinessBase<InterfaceImplementBusinessPOCO>
  {
    private interface IInterfaceImplementBusinessPOCO
    {
      int Id { get; }
      [Display(Name = "Interface Name")]
      string Name { get; set; }
      string? Description { get; set; }
      string? Code { get; }
    }
  }
}
