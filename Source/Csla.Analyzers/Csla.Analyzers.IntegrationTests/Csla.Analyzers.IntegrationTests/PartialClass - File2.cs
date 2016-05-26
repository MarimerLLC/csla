using System;

namespace Csla.Analyzers.IntegrationTests
{
  public partial class PartialClass
    : BusinessBase<PartialClass>
  {
    public static readonly PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(p => p.Id);
    public Guid Id => GetProperty(IdProperty);
  }
}
