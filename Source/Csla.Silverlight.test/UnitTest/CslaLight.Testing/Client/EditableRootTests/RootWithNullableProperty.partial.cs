using System;
using Csla.DataPortalClient;

namespace Csla.Testing.Business.EditableRootTests
{
  public partial class RootWithNullableProperty
  {
    [RunLocal]
    protected override void DataPortal_Create()
    {
      Today = new SmartDate();
      OtherDate = DateTime.Now;
      base.DataPortal_Create();
    }
  }
}
