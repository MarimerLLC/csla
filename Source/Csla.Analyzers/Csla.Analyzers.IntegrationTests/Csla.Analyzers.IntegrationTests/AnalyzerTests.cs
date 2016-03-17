using Csla;
using Csla.Core;
using System;
using System.Threading.Tasks;

[Serializable]
public class ExpressionBodiedMember
  : BusinessBase<ExpressionBodiedMember>
{
  public readonly static PropertyInfo<int> ResourceIdProperty = RegisterProperty<int>(c => c.ResourceId);
  public int ResourceId => ReadProperty(ResourceIdProperty);
}

public interface IBO
  : IBusinessObject
{
  void DataPortal_Create();
}

internal class x : IBO
{
  internal void DataPortal_Create()
  {
    throw new NotImplementedException();
  }
}

public class SomeCriteria
  : CriteriaBase<SomeCriteria>
{ }

public class MyCommandBase
  : CommandBase<MyCommandBase>
{
  public MyCommandBase(int id) { }

  public MyCommandBase()
  {
  }
}

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

  public void SaveItself()
  {
    Save();
  }

  public User SaveItselfAndReturn()
  {
    return Save();
  }

  public async Task SaveItselfAsync()
  {
    await SaveAsync();
  }

  public async Task<User> SaveItselfAndReturnAsync()
  {
    return await SaveAsync();
  }
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

    this.ReturnThis(() => x.Save());

    this.ReturnThis(() => { return x.Save(); });

    this.ReturnThis(() =>
    {
      var q = DataPortal.Fetch<User>();
      // This should have an error because it doesn't set the return value
      q.Save();
      return null;
    });

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

  private User ReturnThis(Func<User> a)
  {
    return a();
  }
}