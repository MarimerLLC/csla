using System.ComponentModel.DataAnnotations;

namespace Csla.Generator.AutoImplementProperties.TestObjects
{
  [CslaImplementPropertiesInterface<IInterfaceImplementBusinessPOCO>]
  public partial class InterfaceImplementBusinessPOCO : BusinessBase<InterfaceImplementBusinessPOCO>
  {
    private interface IInterfaceImplementBusinessPOCO
    {
      int Id { get; }
      [Display(Name = "Interface Name")]
      string Name { get; set; }
      string? Description { get; set; }
      string? Code { get; }

      [CslaIgnoreProperty]
      string IgnoredProperty { get; set; }
    }
  }
}
