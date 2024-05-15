using Csla.Rules;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{
  public class CalculationObject : BusinessBase<CalculationObject>
  {
    public static readonly PropertyInfo<int> PropertyAProperty = RegisterProperty<int>(nameof(PropertyA));
    public int PropertyA
    {
      get { return GetProperty(PropertyAProperty); }
      set { SetProperty(PropertyAProperty, value); }
    }

    public static readonly PropertyInfo<int> PropertyBProperty = RegisterProperty<int>(nameof(PropertyB));
    public int PropertyB
    {
      get { return GetProperty(PropertyBProperty); }
      set { SetProperty(PropertyBProperty, value); }
    }

    public static readonly PropertyInfo<string> SumPropertyProperty = RegisterProperty<string>(nameof(SumProperty));
    public string SumProperty
    {
      get { return GetProperty(SumPropertyProperty); }
      private set { LoadProperty(SumPropertyProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      // Register the sum properties rule
      BusinessRules.AddRule(new CalcSumRule(SumPropertyProperty, PropertyAProperty, PropertyBProperty));
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }
  }

  [TestClass]
  public class CoerceTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void CalcSumRole_SavesToStringProperty()
    {
      // Create an instance of a DataPortal that can be used for instantiating objects
      var dataPortal = _testDIContext.CreateDataPortal<CalculationObject>();
      var obj = dataPortal.Create();

      obj.PropertyA = 10;
      obj.PropertyB = 20;

      Assert.AreEqual("30", obj.SumProperty, "string SumProperty should reflect the sum of PropertyA and PropertyB.");
    }
  }
}
