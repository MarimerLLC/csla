using Csla.Serialization;

namespace Csla.Generator.AutoImplementProperties.TestObjects
{
  [AutoImplementProperties]
  public partial class CommandPOCO : CommandBase<CommandPOCO>
  {
    public partial string CommandName { get; set; }
    public partial string? CommandDescription { get; set; }
    public partial string? CommandCode { get; set; }
  }
}
