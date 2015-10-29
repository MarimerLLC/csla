using Csla;
using System;

[Serializable]
public class OuterClass
  : BusinessBase<OuterClass>
{
  private OuterClass() { }

  [Serializable]
  public class NestedClass
    : BusinessBase<NestedClass>
  {
    private NestedClass() { }
  }
}