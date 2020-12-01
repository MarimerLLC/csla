//-----------------------------------------------------------------------
// <copyright file="UsernameCriteria.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Criteria class for passing a</summary>
//-----------------------------------------------------------------------
using System;
using Csla;

namespace Csla.Security
{
  /// <summary>
  /// Criteria class for passing a
  /// username/password pair to a
  /// custom identity class.
  /// </summary>
  [Serializable]
  public class UsernameCriteria : CriteriaBase<UsernameCriteria>
  {
    /// <summary>
    /// Username property definition.
    /// </summary>
    public static readonly PropertyInfo<string> UsernameProperty = RegisterProperty<string>(c => c.Username);
    /// <summary>
    /// Username property definition.
    /// </summary>
    public string Username
    {
      get { return ReadProperty(UsernameProperty); }
      private set { LoadProperty(UsernameProperty, value); }
    }

    /// <summary>
    /// Password property definition.
    /// </summary>
    public static readonly PropertyInfo<string> PasswordProperty = RegisterProperty<string>(c => c.Password);
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
#if (ANDROID || IOS) || NETFX_CORE
    public UsernameCriteria()
    { }
#else
    protected UsernameCriteria()
    { }
#endif
  }
}