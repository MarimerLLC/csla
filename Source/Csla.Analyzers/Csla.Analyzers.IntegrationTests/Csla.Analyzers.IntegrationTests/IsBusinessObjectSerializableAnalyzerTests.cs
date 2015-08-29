using Csla;
using System;

public class ClassIsStereotypeAndIsNotSerializable
  : BusinessBase<ClassIsStereotypeAndIsNotSerializable>
{ }

public class ClassIsNotStereotype { }

[Serializable]
public class ClassIsStereotypeAndIsSerializable
  : BusinessBase<ClassIsStereotypeAndIsSerializable>
{ }

[Serializable]
public class Foo : BusinessBase<Foo>
{
  public Foo(int x) { }
}