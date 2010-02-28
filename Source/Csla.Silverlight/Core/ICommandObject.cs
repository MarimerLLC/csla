using System;

namespace Csla.Core
{
  /// <summary>
  /// This interface is implemented by all
  /// Command objects.
  /// </summary>
  public interface ICommandObject : IBusinessObject, Csla.Serialization.Mobile.IMobileObject
  {
  }
}
