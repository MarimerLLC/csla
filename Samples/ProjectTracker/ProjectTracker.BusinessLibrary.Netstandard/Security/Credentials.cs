using System;
using System.ComponentModel.DataAnnotations;
using Csla;

namespace ProjectTracker.Library.Security
{
  [Serializable]
  public class Credentials : BusinessBase<Credentials>
  {
    public static readonly PropertyInfo<string> UsernameProperty = RegisterProperty<string>(nameof(Username));
    [Required]
    public string Username
    {
      get => GetProperty(UsernameProperty);
      set => SetProperty(UsernameProperty, value);
    }

    public static readonly PropertyInfo<string> PasswordProperty = RegisterProperty<string>(nameof(Password));
    public string Password
    {
      get => GetProperty(PasswordProperty);
      set => SetProperty(PasswordProperty, value);
    }

    [Create]
    [RunLocal]
    private void Create()
    { }

    [Create]
    [RunLocal]
    private void Create(string username, string password)
    {
      Username = username;
      Password = password;
      BusinessRules.CheckRules();
    }
  }
}
