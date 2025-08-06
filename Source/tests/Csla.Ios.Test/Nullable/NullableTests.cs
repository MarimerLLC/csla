
using System;
using NUnit.Framework;

namespace Csla.Ios.Test
{
  [TestFixture]
  public class NullableTests
  {
    [Test]
    public void TestEditNullableWithValueProperty()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      var entity = NullableEntity.NewNullableEntity();
      entity.NullableInteger = 1;
      Assert.AreEqual(1, entity.NullableInteger);
    }

    [Test]
    public void TestEditNullableWithNullProperty()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      var entity = NullableEntity.NewNullableEntity();
      entity.NullableInteger = 1;
      entity.NullableInteger = null;
      Assert.AreEqual(null, entity.NullableInteger);
    }

    [Test]
    public void TestEditNullableViaBusinessRuleProperty()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      var entity = NullableEntity.NewNullableEntity();
      entity.NullableInteger = 1;
      entity.Name = "test";
      entity.Name = string.Empty;
      Assert.AreEqual(null, entity.NullableInteger);
    }
  }
}
