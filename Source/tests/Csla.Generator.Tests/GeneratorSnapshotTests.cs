using Csla.Generator.AutoImplementProperties.CSharp.AutoImplement;
using Csla.Generator.Tests.Helper;

namespace Csla.Generator.Tests;

public class GeneratorSnapshotTests
{
  public static TheoryData<string> CSharpBuiltInTypes => new(_csharpBuiltInTypes);

  public static TheoryData<string> CSharpBuiltInTypesNullable => new(_csharpBuiltInTypes.Where(t => t is not "string" and not "object"));

  private static readonly string[] _csharpBuiltInTypes = ["string", "bool", "byte", "sbyte", "char", "decimal", "double", "float", "int", "uint", "long", "ulong", "short", "ushort", "object"];



  [Fact]
  public Task BusinessBase()
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

    return SourceGenTester<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(cslaSource, true);
  }

  [Fact]
  public Task CommandBase()
  {
    var cslaSource = @$"
using System.ComponentModel.DataAnnotations;
using Csla.Serialization;
using Csla;

namespace TestObjects
{{
  [CslaImplementProperties]
  public partial class CommandPOCO : CommandBase<CommandPOCO>
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

  public partial class CommandPOCO
  {{
    public partial string IgnoredProperty {{ get => """"; set {{ }} }}
  }}
}}

";

    return SourceGenTester<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(cslaSource, true);
  }

  [Fact]
  public Task ReadOnlyBase()
  {
    var cslaSource = @$"
using System.ComponentModel.DataAnnotations;
using Csla.Serialization;
using Csla;

namespace TestObjects
{{
  [CslaImplementProperties]
  public partial class ReadOnlyPOCO : ReadOnlyBase<ReadOnlyPOCO>
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
  public partial class ReadOnlyPOCO
  {{
    public partial string IgnoredProperty {{ get => """"; set {{ }} }}
  }}
}}

";

    return SourceGenTester<IncrementalAutoImplementPropertiesPartialsGenerator>.Verify(cslaSource, true);
  }

  [Fact]
  public Task BusinessBaseInterface()
  {
    var cslaSource = @$"
using System.ComponentModel.DataAnnotations;
using Csla.Serialization;

namespace Csla.Generator.AutoImplementProperties.TestObjects
{{
  [CslaImplementPropertiesInterface<IInterfaceImplementBusinessPOCO>]
  public partial class InterfaceImplementBusinessPOCO : BusinessBase<InterfaceImplementBusinessPOCO>
  {{
    private interface IInterfaceImplementBusinessPOCO
    {{
      int Id {{ get; }}
      [Display(Name = ""Interface Name"")]
      string Name {{ get; set; }}
      string? Description {{ get; set; }}
      string? Code {{ get; }}

      [CslaIgnoreProperty]
      string IgnoredProperty {{ get; set; }}
    }}
  }}
}}

";

    return SourceGenTester<IncrementalAutoImplementInterfacePartialsGenerator>.Verify(cslaSource, true);
  }

  [Fact]
  public Task CommandBaseInterface()
  {
    var cslaSource = @$"
using System.ComponentModel.DataAnnotations;
using Csla.Serialization;

namespace Csla.Generator.AutoImplementProperties.TestObjects
{{
  [CslaImplementPropertiesInterface<IInterfaceImplementBusinessPOCO>]
  public partial class InterfaceImplementBusinessPOCO : CommandBase<InterfaceImplementBusinessPOCO>
  {{
    private interface IInterfaceImplementBusinessPOCO
    {{
      int Id {{ get; }}
      [Display(Name = ""Interface Name"")]
      string Name {{ get; set; }}
      string? Description {{ get; set; }}
      string? Code {{ get; }}

      [CslaIgnoreProperty]
      string IgnoredProperty {{ get; set; }}
    }}
  }}
}}

";

    return SourceGenTester<IncrementalAutoImplementInterfacePartialsGenerator>.Verify(cslaSource, true);
  }
  [Fact]
  public Task ReadOnlyBaseInterface()
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
      [Display(Name = ""Interface Name"")]
      string Name {{ get; set; }}
      string? Description {{ get; set; }}
      string? Code {{ get; }}

      [CslaIgnoreProperty]
      string IgnoredProperty {{ get; set; }}
    }}
  }}
}}

";

    return SourceGenTester<IncrementalAutoImplementInterfacePartialsGenerator>.Verify(cslaSource, true);
  }
}