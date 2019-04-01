using System;

namespace Csla.Analyzers.IntegrationTests
{
  // This should have an error because it's not serializable,
  // even though the base type is.
  public class SomeCriteria
    : CriteriaBase<SomeCriteria>
  { }

  // This should have an error because it's not serializable,
  // even though the base type is.
  public class MyCommandBase
    : CommandBase<MyCommandBase>
  {
    public MyCommandBase(int id) { }

    public MyCommandBase() { }
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
}