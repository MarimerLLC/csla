using System;
using Csla;
using Csla.Serialization;

namespace ClassLibrary
{
  /// <summary>
  /// Criteria class for passing a
  /// username/password pair to a
  /// custom identity class.
  /// </summary>
  [Serializable]
  public class UsernameCriteria : CriteriaBase
  {
    private static PropertyInfo<string> UsernameProperty =
      RegisterProperty(typeof(UsernameCriteria), new PropertyInfo<string>("Username", "Username"));
    /// <summary>
    /// Gets the username.
    /// </summary>
    public string Username
    {
      get { return ReadProperty(UsernameProperty); }
      private set { LoadProperty(UsernameProperty, value); }
    }

    private static PropertyInfo<string> PasswordProperty =
      RegisterProperty(typeof(UsernameCriteria), new PropertyInfo<string>("Password", "Password"));
    /// <summary>
    /// Gets the password.
    /// </summary>
    public string Password
    {
      get { return ReadProperty(PasswordProperty); }
      private set { LoadProperty(PasswordProperty, value); }
    }

    public UsernameCriteria()
    { }

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