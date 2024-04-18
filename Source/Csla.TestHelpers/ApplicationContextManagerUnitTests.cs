﻿namespace Csla.TestHelpers
{
  public class ApplicationContextManagerUnitTests : Csla.Core.ApplicationContextManagerAsyncLocal
  {
    public Guid InstanceId { get; private set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; private set; } = DateTime.Now;
  }
}
