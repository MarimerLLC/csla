using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;
using Csla.Generator.Tests.Helper;

namespace Csla.Generator.Tests;

public class SourceGeneratorStageCachingTests
{
  [Fact]
  public void EachCslaPortalOperationStageIsCachedCorrectlyInterface()
  {
    var cslaSource = @$"
using System.ComponentModel.DataAnnotations;
using Csla.Serialization;

namespace Csla.Generator.AutoImplementProperties.TestObjects
{{
  [CslaImplementPropertiesInterface<IInterfaceImplementBusinessPOCO>]
  public partial class InterfaceImplementBusinessPOCO : ReadOnlyBase<InterfaceImplementBusinessPOCO>
  {{
    private interface IInterfaceImplementBusinessPOCO
    {{
      int Id {{ get; }}
      string Name {{ get; set; }}
      string? Description {{ get; set; }}
      string? Code {{ get; }}
      string IgnoredProperty {{ get; set; }}
    }}
  }}
}}

";

    StageCachingTester<IncrementalAutoImplementInterfacePartialsGenerator>.VerfiyStageCaching(cslaSource);
  }

  [Fact]
  public void EachCslaPortalOperationStageIsCachedCorrectlyPartialProperty()
  {
    var cslaSource = @$"
using System.ComponentModel.DataAnnotations;
using Csla.Serialization;
using Csla;

namespace TestObjects
{{
  [CslaImplementProperties]
  public partial class AddressPOCO : BusinessBase<AddressPOCO>
  {{
    [Display(Name = ""Address Line 1"")]
    public partial string? AddressLine1 {{ get; private set; }}
    public partial string AddressLine2 {{ get; set; }}
    public partial string Town {{ get; set; }}
    public partial string County {{ get; set; }}
    public partial string Postcode {{ get; set; }}
    [CslaIgnoreProperty]
    public partial string IgnoredProperty {{ get; set; }}
  }}

  public partial class AddressPOCO
  {{
    public partial string IgnoredProperty {{ get => """"; set {{ }} }}
  }}
}}


";

    StageCachingTester<IncrementalAutoImplementPropertiesPartialsGenerator>.VerfiyStageCaching(cslaSource);
  }
}
