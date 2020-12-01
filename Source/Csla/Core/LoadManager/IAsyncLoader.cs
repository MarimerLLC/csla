using System;

namespace Csla.Core.LoadManager
{
  internal interface IAsyncLoader
  {
    IPropertyInfo Property { get; }
    void Load(Action<IAsyncLoader, IDataPortalResult> callback);
  }
}