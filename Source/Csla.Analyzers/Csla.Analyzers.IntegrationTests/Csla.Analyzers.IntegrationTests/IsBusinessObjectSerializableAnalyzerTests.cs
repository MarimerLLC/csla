using Csla;
using System;
using System.Threading.Tasks;

public class ClassIsStereotypeAndIsNotSerializable
  : BusinessBase<ClassIsStereotypeAndIsNotSerializable>
{ }

public class ClassIsNotStereotype { }

[Serializable]
public class ClassIsStereotypeAndIsSerializable
  : BusinessBase<ClassIsStereotypeAndIsSerializable>
{ }

[Serializable]
public class Foo
  : BusinessBase<Foo>
{
  private Foo(int x) { }
}

public class x
{
  private Foo f;

  public x Save() { return null; }

  public async Task<x> SaveAsync() { return await Task.FromResult<x>(null); }

  public async Task fooAsync()
  {
    // Case 1
    var x = DataPortal.Fetch<Foo>();
    await x.SaveAsync();
    await this.SaveAsync();
    await x.SaveAsync(true);

    // Case 2
    var y = DataPortal.Fetch<Foo>();
    y = await y.SaveAsync();

    // Case 3
    var z = DataPortal.Fetch<Foo>();
    var a = await z.SaveAsync();

    // Case 4
    f = await z.SaveAsync();

    // Case 5
    this.DoThis(() => f = z.Save());

    // Case 6
    this.DoThis(() => { f = z.Save(); z.Save(); });

    Func<Foo> runThis = x.Save;
    runThis();
  }

  public void foo()
  {
    // Case 1
    var x = DataPortal.Fetch<Foo>();
    x.Save();
    this.Save();
    x.Save(true);

    // Case 2
    var y = DataPortal.Fetch<Foo>();
    y = y.Save();

    // Case 3
    var z = DataPortal.Fetch<Foo>();
    var a = z.Save();

    // Case 4
    f = z.Save();

    // Case 5
    this.DoThis(() => f = z.Save());

    // Case 6
    this.DoThis(() => { f = z.Save(); z.Save(); });

    Func<Foo> runThis = x.Save;
    runThis();
  }

  private void DoThis(Action a)
  {
    a();
  }
}