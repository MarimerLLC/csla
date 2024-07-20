using Csla.Serialization;

namespace Csla.Generator.AutoImplementProperties.TestObjects
{
  [AutoImplementProperties]
  public partial class Command : CommandBase<Command>
  {
    public partial string CommandName { get; set; }
    public partial string? CommandDescription { get; set; }
    public partial string? CommandCode { get; set; }
  }
}
