using System;

namespace Csla
{
  /// <summary>
  /// Marks a DataPortal_XYZ method to
  /// be run on the client even if the server-side
  /// DataPortal is configured for remote use.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class RunLocalAttribute : Attribute
  {

  }
}
