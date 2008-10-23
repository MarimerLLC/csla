using System;

namespace Csla.Security
{
  /// <summary>
  /// Criteria class for passing a
  /// username/password pair to a
  /// custom identity class.
  /// </summary>
  [Serializable]
  public class UsernameCriteria
  {
    /// <summary>
    /// Gets the username.
    /// </summary>
    public string Username { get; private set; }
    /// <summary>
    /// Gets the password.
    /// </summary>
    public string Password { get; private set; }

    /// <summary>
    /// Creates a new instance of the object.
    /// </summary>
    /// <param name="username">
    /// Username value.
    /// </param>
    /// <param name="password">
    /// Password value.
    /// </param>
    public UsernameCriteria(string username, string password)
    {
      this.Username = username;
      this.Password = password;
    }
  }
}