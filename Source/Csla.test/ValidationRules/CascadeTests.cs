

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{
  [TestClass]
  public class CascadeTests
  {
    [TestMethod]
    public void BusinessRules_MustCascade_WhenCascadeOnDirtyPropertiesIsTrue()
    {
      var root = new CascadeRoot {CascadeOnDirtyProperties = true};
      Assert.AreEqual(0, root.Num1);
      Assert.AreEqual(0, root.Num2);
      Assert.AreEqual(0, root.Num3);
      Assert.AreEqual(0, root.Num4);
      root.Num1 = 1;
      Assert.AreEqual(1, root.Num1); // the value set by test 
      Assert.AreEqual(2, root.Num2); // Num2 is set from Num1 rules 
      Assert.AreEqual(3, root.Num3); // Num3 is set from Num2 rules
      Assert.AreEqual(4, root.Num4); // Num4 is set from Num3 rules when Cascade propagates down to Num3
    }

    [TestMethod]
    public void BusinessRules_MustNotCascade_WhenCanRunAsAffectedIsFalse()
    {
      var root = new CascadeRoot { CascadeOnDirtyProperties = true };
      Assert.AreEqual(0, root.Num1);
      Assert.AreEqual(0, root.Num2);
      Assert.AreEqual(0, root.Num3);
      Assert.AreEqual(0, root.Num4);
      Assert.AreEqual(0, root.Num5);
      root.Num1 = 1;
      Assert.AreEqual(1, root.Num1); // the value set by test 
      Assert.AreEqual(2, root.Num2); // Num2 is set from Num1 rules 
      Assert.AreEqual(3, root.Num3); // Num3 is set from Num2 rules
      Assert.AreEqual(4, root.Num4); // Num4 is set from Num3 rules when Cascade propagates down to Num3
      Assert.AreEqual(0, root.Num5); // Rule on Num4 is not allowed to run as AffectedProperty som Num5 must be unchanged.
    }

    [TestMethod]
    public void BusinessRules_MustNotCascade_WhenCascadeOnDirtyPropertiesIsFalse()
    {
      var root = new CascadeRoot {CascadeOnDirtyProperties = false};
      Assert.AreEqual(0, root.Num1);
      Assert.AreEqual(0, root.Num2);
      Assert.AreEqual(0, root.Num3);
      Assert.AreEqual(0, root.Num4);
      root.Num1 = 1;
      Assert.AreEqual(1, root.Num1); // the value set by test
      Assert.AreEqual(2, root.Num2); // rerun rules for Num2 without cascade
      Assert.AreEqual(3, root.Num3); // will set Num3 as output from Num2 but not rerun rules for Num3
      Assert.AreEqual(0, root.Num4); // Num4 is unchanged
    }
  }
}