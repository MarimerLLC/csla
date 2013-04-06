using System;

namespace Csla.Security
{
  /// <summary>
  /// Interface defining an object that
  /// checks IsInRole.
  /// </summary>
  public interface ICheckRoles
  {
    /// <summary>
    /// Returns a value indicating whether the current
    /// user is in the specified security role.
    /// </summary>
    /// <param name="role">
    /// Role to check.
    /// </param>
    bool IsInRole(string role);
  }
}
