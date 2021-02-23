using System;

namespace Csla.Analyzers.IntegrationTests
{
  // This should have an error because it doesn't have a public constructor.
  [Serializable]
  public class OuterClass
  : BusinessBase<OuterClass>
  {
    private OuterClass() { }

    // This should have an error because it doesn't have a public constructor
    // (note that it's a nested class).
    [Serializable]
    public class NestedClass
      : BusinessBase<NestedClass>
    {
      private NestedClass() { }
    }
  }

  // This should have an error because it doesn't have a public constructor
  // and a warning for the constructor with arguments.
  [Serializable]
  public class PrivateConstructorTest
    : BusinessBase<PrivateConstructorTest>
  {
    private PrivateConstructorTest(int x) { }
  }

  // This should have an error because it has a public constructor
  // with arguments.
  [Serializable]
  public class PublicConstructorWithArgumentsTest
    : BusinessBase<PublicConstructorWithArgumentsTest>
  {
    public PublicConstructorWithArgumentsTest(int x) { }
  }

  // This should have an error because it doesn't have a public constructor...
  [Serializable]
  public class InternalConstructorTest
    : BusinessBase<PrivateConstructorTest>
  {
    // ... and I don't want to lose the comment
    InternalConstructorTest() { }
  }

  // This should have an error because it doesn't have a public constructor...
  [Serializable]
  public class PrivateConstructorTestNoArguments
    : BusinessBase<PrivateConstructorTest>
  {
    // ... and I don't want to lose the comment
    private PrivateConstructorTestNoArguments() { }
  }

  [Serializable]
  public class PublicConstructorExplicitNoArgumentTest
    : BusinessBase<PublicConstructorExplicitNoArgumentTest>
  {
    public PublicConstructorExplicitNoArgumentTest() { }
  }

  [Serializable]
  public class PublicConstructorTest
    : BusinessBase<PublicConstructorTest>
  { }
}