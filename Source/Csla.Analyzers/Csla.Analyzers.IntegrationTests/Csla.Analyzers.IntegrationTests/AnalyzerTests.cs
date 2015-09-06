using Csla;
using System;
using System.Threading.Tasks;

// This should have an error because it's not serializable
public class ClassIsStereotypeAndIsNotSerializable
  : BusinessBase<ClassIsStereotypeAndIsNotSerializable>
{ }

public class ClassIsNotStereotype { }

[Serializable]
public class ClassIsStereotypeAndIsSerializable
  : BusinessBase<ClassIsStereotypeAndIsSerializable>
{ }

// This should have an error because it doesn't have a public constructor
// and a warning for the constructor with arguments.
[Serializable]
public class User
  : BusinessBase<User>
{
  private User(int x) { }
}

public class UserCaller
{
  private User value;

  public UserCaller Save() { return null; }

  public async Task<UserCaller> SaveAsync() { return await Task.FromResult<UserCaller>(null); }

  public async Task UserSaveAsync()
  {
    var x = DataPortal.Fetch<User>();

    // This should have an error because it doesn't set the return value
    await x.SaveAsync();

    await this.SaveAsync();
    
    // This should have an error because it doesn't set the return value
    await x.SaveAsync(true);

    x = await x.SaveAsync();

    var a = await x.SaveAsync();

    this.value = await x.SaveAsync();

    await DataPortal.Fetch<User>().SaveAsync();
  }

  public void UserSave()
  {
    var x = DataPortal.Fetch<User>();

    // This should have an error because it doesn't set the return value
    x.Save();

    this.Save();

    // This should have an error because it doesn't set the return value
    x.Save(true);

    x = x.Save();

    var a = x.Save();

    x.Save(true);

    this.value = x.Save();

    this.DoThis(() => { this.value = x.Save(); });

    this.DoThis(() => this.value = x.Save());

    // This should have an error because it doesn't set the return value
    this.DoThis(() => { x.Save(); });

    DataPortal.Fetch<User>().Save();
  }

  public User ReturnsUser()
  {
    var x = DataPortal.Fetch<User>();
    return x.Save();
  }

  public async Task<User> ReturnsUserAsync()
  {
    var x = DataPortal.Fetch<User>();
    return await x.SaveAsync();
  }

  private void DoThis(Action a)
  {
    a();
  }
}