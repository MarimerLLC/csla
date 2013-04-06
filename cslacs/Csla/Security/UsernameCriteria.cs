using System;
using Csla;
using Csla.Serialization;

namespace Csla.Security
{
  /// <summary>
  /// Criteria class for passing a
  /// username/password pair to a
  /// custom identity class.
  /// </summary>
  [Serializable]
  public class UsernameCriteria : CriteriaBase
  {
    /// <summary>
    /// Username property definition.
    /// </summary>
    public static PropertyInfo<string> UsernameProperty = RegisterProperty(typeof(UsernameCriteria), new PropertyInfo<string>("Username", "Username"));
    /// <summary>
    /// Gets the username.
    /// </summary>
    public string Username
    {
      get { return ReadProperty(UsernameProperty); }
      private set { LoadProperty(UsernameProperty, value); }
    }

    /// <summary>
    /// Password property definition.
    /// </summary>
    public static PropertyInfo<string> PasswordProperty = RegisterProperty(typeof(UsernameCriteria), new PropertyInfo<string>("Password", "Password"));
    /// <summary>
    /// Gets the password.
    /// </summary>
    public string Password
    {
      get { return ReadProperty(PasswordProperty); }
      private set { LoadProperty(PasswordProperty, value); }
    }

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

    /// <summary>
    /// Creates a new instance of the object.
    /// </summary>
#if SILVERLIGHT
    public UsernameCriteria()
    { }
#else
    protected UsernameCriteria()
    { }
#endif
  }
}