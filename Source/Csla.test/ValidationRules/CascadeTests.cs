

using System.Linq;
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

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void BusinessRules_MustCascadeAsSpreadsheet_WhenCascadeOnDirtyPropertiesIsTrue()
    {
      // complex ruleset 
      // calculate the sum of Aa and Ab to Ac
      // copy value of Ac to Ae
      // calculate sum of Ad and Ae to Af
      // copy value of Af to Ag
      var root = new CascadeRoot { CascadeOnDirtyProperties = true };
      root.CheckRules();
      Assert.AreEqual(0, root.ValueAc); 
      Assert.AreEqual(0, root.ValueAg);
      Assert.AreEqual(0, root.ValueAe);
      Assert.AreEqual(0, root.ValueAf);

      root.ValueAa = 10;
      Assert.AreEqual(10, root.ValueAc);
      Assert.AreEqual(10, root.ValueAg);
      Assert.AreEqual(10, root.ValueAe);
      Assert.AreEqual(10, root.ValueAf);

      root.ValueAb = 20;
      Assert.AreEqual(30, root.ValueAc); 
      Assert.AreEqual(30, root.ValueAe);
      Assert.AreEqual(30, root.ValueAf);
      Assert.AreEqual(30, root.ValueAg);

      root.ValueAd = 25;
      Assert.AreEqual(55, root.ValueAf);
      Assert.AreEqual(55, root.ValueAg);
    }


    [TestMethod]
    
    public void BusinessRules_MustCheckBothSums_WhenCascadeOnDirtyPropertiesIsTrue()
    {
      // check that the sum of Ba and Bb is always 100 (and error message on both properties)
      var root = new CascadeRoot { CascadeOnDirtyProperties = true };
      root.CheckRules();
      Assert.AreEqual(0, root.ValueBa);
      Assert.AreEqual(0, root.ValueBb);
      Assert.IsTrue(root.BrokenRulesCollection.Any(p => p.Property == CascadeRoot.ValueBaProperty.Name));
      Assert.IsTrue(root.BrokenRulesCollection.Any(p => p.Property == CascadeRoot.ValueBbProperty.Name));

      root.ValueBa = 100;
      Assert.IsFalse(root.BrokenRulesCollection.Any(p => p.Property == CascadeRoot.ValueBaProperty.Name));
      Assert.IsFalse(root.BrokenRulesCollection.Any(p => p.Property == CascadeRoot.ValueBbProperty.Name));

      root.ValueBb = 50;
      Assert.IsTrue(root.BrokenRulesCollection.Any(p => p.Property == CascadeRoot.ValueBaProperty.Name));
      Assert.IsTrue(root.BrokenRulesCollection.Any(p => p.Property == CascadeRoot.ValueBbProperty.Name));

      root.ValueBa = 50;
      Assert.IsFalse(root.BrokenRulesCollection.Any(p => p.Property == CascadeRoot.ValueBaProperty.Name));
      Assert.IsFalse(root.BrokenRulesCollection.Any(p => p.Property == CascadeRoot.ValueBbProperty.Name));
    }

    [TestMethod]
    
    public void BusinessRules_MustRecalculateSumAfterCaclulateFraction_WhenCascadeOnDirtyPropertiesIsTrue()
    {
      // calculate sum of Ca, Cb, Cc and Cd to Ce
      // calculate fraction of Ce to Cd
      // must then recalculate sum again as Cd was changed.
      var root = new CascadeRoot { CascadeOnDirtyProperties = true };
      root.CheckRules();
      Assert.AreEqual(0, root.ValueCa);
      Assert.AreEqual(0, root.ValueCb);
      Assert.AreEqual(0, root.ValueCc);
      Assert.AreEqual(0, root.ValueCd);
      Assert.AreEqual(0, root.ValueCe);

      root.ValueCa = 5;
      Assert.AreEqual(5, root.ValueCa);
      Assert.AreEqual(0, root.ValueCb);
      Assert.AreEqual(0, root.ValueCc);
      Assert.AreEqual(1.67m, root.ValueCd);
      Assert.AreEqual(6.67m, root.ValueCe);

      root.ValueCb = 15;
      Assert.AreEqual(5, root.ValueCa);
      Assert.AreEqual(15, root.ValueCb);
      Assert.AreEqual(0, root.ValueCc);
      Assert.AreEqual(6.67m, root.ValueCd);
      Assert.AreEqual(26.67m, root.ValueCe);

      root.ValueCc = 25;
      Assert.AreEqual(5, root.ValueCa);
      Assert.AreEqual(15, root.ValueCb);
      Assert.AreEqual(25, root.ValueCc);
      Assert.AreEqual(15.00m, root.ValueCd);
      Assert.AreEqual(60.00m, root.ValueCe);
    }
  }
}