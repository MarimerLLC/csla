//-----------------------------------------------------------------------
// <copyright file="ValidationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Csla.Core;
using Csla.Rules;
using UnitDriven;
using Csla.Serialization;
using System.Threading.Tasks;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.ValidationRules
{
  [TestClass()]
  public class ValidationTests : TestBase
  {

    [TestMethod()]
    public async Task TestValidationRulesWithPrivateMember()
    {
      //works now because we are calling ValidationRules.CheckRules() in DataPortal_Create
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.GlobalContext.Clear();
      var root = await Csla.DataPortal.CreateAsync<HasRulesManager>(new HasRulesManager.Criteria());
      context.Assert.AreEqual("<new>", root.Name);
      context.Assert.AreEqual(true, root.IsValid, "should be valid on create");
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);

      root.BeginEdit();
      root.Name = "";
      root.CancelEdit();

      context.Assert.AreEqual("<new>", root.Name);
      context.Assert.AreEqual(true, root.IsValid, "should be valid after CancelEdit");
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);

      root.BeginEdit();
      root.Name = "";
      root.ApplyEdit();

      context.Assert.AreEqual("", root.Name);
      context.Assert.AreEqual(false, root.IsValid);
      context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
      context.Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);
      context.Assert.Success();

      context.Complete();
    }

    [TestMethod()]
    public async Task TestValidationRulesWithPublicProperty()
    {
      //should work since ValidationRules.CheckRules() is called in DataPortal_Create
      Csla.ApplicationContext.GlobalContext.Clear();
      var root = await Csla.DataPortal.CreateAsync<HasRulesManager2>(new HasRulesManager2.Criteria("<new>"));
      Assert.AreEqual("<new>", root.Name);
      Assert.AreEqual(true, root.IsValid, "should be valid on create");
      Assert.AreEqual(0, root.BrokenRulesCollection.Count);

      root.BeginEdit();
      root.Name = "";
      root.CancelEdit();

      Assert.AreEqual("<new>", root.Name);
      Assert.AreEqual(true, root.IsValid, "should be valid after CancelEdit");
      Assert.AreEqual(0, root.BrokenRulesCollection.Count);

      root.BeginEdit();
      root.Name = "";
      root.ApplyEdit();

      Assert.AreEqual("", root.Name);
      Assert.AreEqual(false, root.IsValid);
      Assert.AreEqual(1, root.BrokenRulesCollection.Count);
      Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);
      Assert.AreEqual("Name required", root.BrokenRulesCollection.GetFirstMessage(HasRulesManager2.NameProperty).Description);
      Assert.AreEqual("Name required", root.BrokenRulesCollection.GetFirstBrokenRule(HasRulesManager2.NameProperty).Description);
    }

    [TestMethod()]
    public async Task TestValidationAfterEditCycle()
    {
      //should work since ValidationRules.CheckRules() is called in DataPortal_Create
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();
      var root = await Csla.DataPortal.CreateAsync<HasRulesManager>(new HasRulesManager.Criteria());
      context.Assert.AreEqual("<new>", root.Name);
      context.Assert.AreEqual(true, root.IsValid, "should be valid on create");
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);

      bool validationComplete = false;
      root.ValidationComplete += (vo, ve) => { validationComplete = true; };

      root.BeginEdit();
      root.Name = "";
      context.Assert.AreEqual("", root.Name);
      context.Assert.AreEqual(false, root.IsValid);
      context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
      context.Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);
      context.Assert.IsTrue(validationComplete, "ValidationComplete should have run");
      root.BeginEdit();
      root.Name = "Begin 1";
      context.Assert.AreEqual("Begin 1", root.Name);
      context.Assert.AreEqual(true, root.IsValid);
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
      root.BeginEdit();
      root.Name = "Begin 2";
      context.Assert.AreEqual("Begin 2", root.Name);
      context.Assert.AreEqual(true, root.IsValid);
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
      root.BeginEdit();
      root.Name = "Begin 3";
      context.Assert.AreEqual("Begin 3", root.Name);
      context.Assert.AreEqual(true, root.IsValid);
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);

      HasRulesManager hrmClone = root.Clone();

      //Test validation rule cancels for both clone and cloned
      root.CancelEdit();
      context.Assert.AreEqual("Begin 2", root.Name);
      context.Assert.AreEqual(true, root.IsValid);
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
      hrmClone.CancelEdit();
      context.Assert.AreEqual("Begin 2", hrmClone.Name);
      context.Assert.AreEqual(true, hrmClone.IsValid);
      context.Assert.AreEqual(0, hrmClone.BrokenRulesCollection.Count);
      root.CancelEdit();
      context.Assert.AreEqual("Begin 1", root.Name);
      context.Assert.AreEqual(true, root.IsValid);
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
      hrmClone.CancelEdit();
      context.Assert.AreEqual("Begin 1", hrmClone.Name);
      context.Assert.AreEqual(true, hrmClone.IsValid);
      context.Assert.AreEqual(0, hrmClone.BrokenRulesCollection.Count);
      root.CancelEdit();
      context.Assert.AreEqual("", root.Name);
      context.Assert.AreEqual(false, root.IsValid);
      context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
      context.Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);
      hrmClone.CancelEdit();
      context.Assert.AreEqual("", hrmClone.Name);
      context.Assert.AreEqual(false, hrmClone.IsValid);
      context.Assert.AreEqual(1, hrmClone.BrokenRulesCollection.Count);
      context.Assert.AreEqual("Name required", hrmClone.BrokenRulesCollection[0].Description);
      root.CancelEdit();
      context.Assert.AreEqual("<new>", root.Name);
      context.Assert.AreEqual(true, root.IsValid);
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
      hrmClone.CancelEdit();
      context.Assert.AreEqual("<new>", hrmClone.Name);
      context.Assert.AreEqual(true, hrmClone.IsValid);
      context.Assert.AreEqual(0, hrmClone.BrokenRulesCollection.Count);
      context.Assert.Success();

      context.Complete();
    }

    [TestMethod()]
    public async Task TestValidationRulesAfterClone()
    {
      //this test uses HasRulesManager2, which assigns criteria._name to its public
      //property in DataPortal_Create.  If it used HasRulesManager, it would fail
      //the first assert, but pass the others
      Csla.ApplicationContext.GlobalContext.Clear();
      var root = await Csla.DataPortal.CreateAsync<HasRulesManager2>(new HasRulesManager2.Criteria("test"));
      Assert.AreEqual(true, root.IsValid);
      root.BeginEdit();
      root.Name = "";
      root.ApplyEdit();

      Assert.AreEqual(false, root.IsValid);
      HasRulesManager2 rootClone = root.Clone();
      Assert.AreEqual(false, rootClone.IsValid);

      rootClone.Name = "something";
      Assert.AreEqual(true, rootClone.IsValid);
    }

    [TestMethod()]

    public async Task BreakRequiredRule()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      var root = await Csla.DataPortal.CreateAsync<HasRulesManager>(new HasRulesManager.Criteria());
      root.Name = "";
      Assert.AreEqual(false, root.IsValid, "should not be valid");
      Assert.AreEqual(1, root.BrokenRulesCollection.Count);
      Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);
    }

    [TestMethod()]

    public async Task BreakLengthRule()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();
      var root = await Csla.DataPortal.CreateAsync<HasRulesManager>(new HasRulesManager.Criteria());
      root.Name = "12345678901";
      context.Assert.AreEqual(false, root.IsValid, "should not be valid");
      context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
      //Assert.AreEqual("Name too long", root.GetBrokenRulesCollection[0].Description);
      Assert.AreEqual("Name can not exceed 10 characters", root.BrokenRulesCollection[0].Description);

      root.Name = "1234567890";
      context.Assert.AreEqual(true, root.IsValid, "should be valid");
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod()]

    public async Task BreakLengthRuleAndClone()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();
      var root = await Csla.DataPortal.CreateAsync<HasRulesManager>(new HasRulesManager.Criteria());
      root.Name = "12345678901";
      context.Assert.AreEqual(false, root.IsValid, "should not be valid before clone");
      context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
      //Assert.AreEqual("Name too long", root.GetBrokenRulesCollection[0].Description;
      Assert.AreEqual("Name can not exceed 10 characters", root.BrokenRulesCollection[0].Description);

      root = (HasRulesManager)(root.Clone());
      context.Assert.AreEqual(false, root.IsValid, "should not be valid after clone");
      context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
      //Assert.AreEqual("Name too long", root.GetBrokenRulesCollection[0].Description;
      context.Assert.AreEqual("Name can not exceed 10 characters", root.BrokenRulesCollection[0].Description);

      root.Name = "1234567890";
      context.Assert.AreEqual(true, root.IsValid, "Should be valid");
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod()]
    public void RegExSSN()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();

      HasRegEx root = new HasRegEx();

      root.Ssn = "555-55-5555";
      root.Ssn2 = "555-55-5555";
      context.Assert.IsTrue(root.IsValid, "Ssn should be valid");

      root.Ssn = "555-55-5555d";
      context.Assert.IsFalse(root.IsValid, "Ssn should not be valid");

      root.Ssn = "555-55-5555";
      root.Ssn2 = "555-55-5555d";
      context.Assert.IsFalse(root.IsValid, "Ssn should not be valid");

      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MergeBrokenRules()
    {
      UnitTestContext context = GetContext();
      var root = new BrokenRulesMergeRoot();
      root.Validate();
      Csla.Rules.BrokenRulesCollection list = root.BrokenRulesCollection;
      context.Assert.AreEqual(2, list.Count, "Should have 2 broken rules");
      context.Assert.AreEqual("rule://csla.test.validationrules.brokenrulesmergeroot-rulebroken/Test1", list[0].RuleName);

      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public async Task VerifyUndoableStateStackOnClone()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      using (UnitTestContext context = GetContext())
      {
        var root = await Csla.DataPortal.CreateAsync<HasRulesManager2>();
        string expected = root.Name;
        root.BeginEdit();
        root.Name = "";
        HasRulesManager2 rootClone = root.Clone();
        rootClone.CancelEdit();

        string actual = rootClone.Name;
        context.Assert.AreEqual(expected, actual);
        context.Assert.Try(rootClone.ApplyEdit);

        context.Assert.Success();
        context.Complete();
      }
    }

    [TestMethod()]
    public async Task ListChangedEventTrigger()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();
      var root = await Csla.DataPortal.CreateAsync<HasChildren>();
      context.Assert.AreEqual(false, root.IsValid);
      root.BeginEdit();
      root.ChildList.Add(Csla.DataPortal.CreateChild<Child>());
      context.Assert.AreEqual(true, root.IsValid);

      root.CancelEdit();
      context.Assert.AreEqual(false, root.IsValid);

      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void RuleThrowsException()
    {
      UnitTestContext context = GetContext();
      var root = new HasBadRule();
      root.Validate();
      context.Assert.IsFalse(root.IsValid);
      context.Assert.AreEqual(1, root.GetBrokenRules().Count);
#if WINDOWS_PHONE
      context.Assert.AreEqual("rule://csla.test.validationrules.hasbadrule-badrule/null:InvalidOperationException", root.GetBrokenRules()[0].Description);
#else
      context.Assert.AreEqual("rule://csla.test.validationrules.hasbadrule-badrule/(object):Operation is not valid due to the current state of the object.", root.GetBrokenRules()[0].Description);
#endif
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void PrivateField()
    {
      UnitTestContext context = GetContext();
      var root = new HasPrivateFields();
      root.Validate();
      context.Assert.IsFalse(root.IsValid);
      root.Name = "abc";
      context.Assert.IsTrue(root.IsValid);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MinMaxValue()
    {
      var context = GetContext();
      var root = Csla.DataPortal.Create<UsesCommonRules>();
      context.Assert.AreEqual(1, root.Data);

      context.Assert.IsFalse(root.IsValid);
      context.Assert.IsTrue(root.BrokenRulesCollection[0].Description.Length > 0);


      root.Data = 0;
      context.Assert.IsFalse(root.IsValid);

      root.Data = 20;
      context.Assert.IsFalse(root.IsValid);

      root.Data = 15;
      context.Assert.IsTrue(root.IsValid);

      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MinMaxNullableValue()
    {
      var context = GetContext();
      var root = Csla.DataPortal.Create<MinMaxNullableRules>();
      context.Assert.IsNull(root.DataNullable);

      context.Assert.IsFalse(root.IsValid);
      context.Assert.IsTrue(root.BrokenRulesCollection[0].Description.Length > 0);


      root.DataNullable = 0;
      context.Assert.IsFalse(root.IsValid);

      root.DataNullable = 20;
      context.Assert.IsFalse(root.IsValid);

      root.DataNullable = 15;
      context.Assert.IsTrue(root.IsValid);

      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void MinMaxLength()
    {
      var context = GetContext();

      var root = Csla.DataPortal.Create<UsesCommonRules>();
      root.Data = 15;
      context.Assert.IsTrue(root.IsValid, "Should start valid");

      root.MinCheck = "a";
      context.Assert.IsFalse(root.IsValid, "Min too short");

      root.MinCheck = "123456";
      context.Assert.IsTrue(root.IsValid, "Min OK");

      root.MaxCheck = "a";
      context.Assert.IsTrue(root.IsValid, "Max OK");

      root.MaxCheck = "123456";
      context.Assert.IsFalse(root.IsValid, "Max too long");

      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void TwoRules()
    {
      var context = GetContext();

      var root = new TwoPropertyRules();
      var rule = new TwoProps(TwoPropertyRules.Value1Property, TwoPropertyRules.Value2Property);

      var ctx = new Csla.Rules.RuleContext(null, rule, root,
        new Dictionary<Core.IPropertyInfo, object> { 
          { TwoPropertyRules.Value1Property, "a" },
          { TwoPropertyRules.Value2Property, "b" } 
        });
      ((Csla.Rules.IBusinessRule)rule).Execute(ctx);
      context.Assert.AreEqual(0, ctx.Results.Count);

      ctx = new Csla.Rules.RuleContext(null, rule, root,
        new Dictionary<Core.IPropertyInfo, object> { 
          { TwoPropertyRules.Value1Property, "" },
          { TwoPropertyRules.Value2Property, "a" } 
        });
      ((Csla.Rules.IBusinessRule)rule).Execute(ctx);
      context.Assert.AreEqual(1, ctx.Results.Count);

      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void CanHaveRuleOnLazyField()
    {
      var context = GetContext();

      context.Assert.Try(() =>
        {
          var root = new HasLazyField();
          root.CheckRules();

          string broken;

          var rootEI = (IDataErrorInfo)root;
          broken = rootEI[HasLazyField.Value1Property.Name];

          context.Assert.AreEqual("PrimaryProperty does not exist.", broken);
          var value = root.Value1;  // intializes field
          
          root.CheckRules();
          broken = rootEI[HasLazyField.Value1Property.Name];
          context.Assert.AreEqual("PrimaryProperty has value.", broken);

          context.Assert.Success();
        });

      context.Complete();

    }

    [TestMethod]
    public void ObjectDirtyWhenOutputValueChangesPropertyValue()
    {
      var context = GetContext();

      var root = new DirtyAfterOutValueChangesProperty();
      context.Assert.IsFalse(root.IsDirty);
      context.Assert.AreEqual("csla rocks", root.Value1);
      root.CheckRules();
      context.Assert.IsTrue(root.IsDirty);
      context.Assert.AreEqual("CSLA ROCKS", root.Value1);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void ObjectNotDirtyWhenOutputValueDoNotChangePropertyValue()
    {
      var context = GetContext();

      var root = new DirtyAfterOutValueChangesProperty("CSLA ROCKS");
      context.Assert.IsFalse(root.IsDirty);
      context.Assert.AreEqual("CSLA ROCKS", root.Value1);
      root.CheckRules();
      context.Assert.IsFalse(root.IsDirty);
      context.Assert.AreEqual("CSLA ROCKS", root.Value1);
      context.Assert.Success();
      context.Complete();
    }
  }

  [Serializable]
  public class HasBadRule : BusinessBase<HasBadRule>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }

    public new Rules.BrokenRulesCollection GetBrokenRules()
    {
      return BusinessRules.GetBrokenRules();
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new BadRule());
    }

    private class BadRule : Rules.BusinessRule
    {
      protected override void Execute(Rules.IRuleContext context)
      {
        throw new InvalidOperationException();
      }
    }
  }

  [Serializable]
  public class HasPrivateFields : BusinessBase<HasPrivateFields>
  {
    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name), RelationshipTypes.PrivateField);
    private string _name = NameProperty.DefaultValue;
    public string Name
    {
      get { return GetProperty(NameProperty, _name); }
      set { SetProperty(NameProperty, ref _name, value); }
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }

    protected override object ReadProperty(Core.IPropertyInfo propertyInfo)
    {
      if (ReferenceEquals(propertyInfo, NameProperty))
        return _name;
      else
        return base.ReadProperty(propertyInfo);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
    }
  }

  [Serializable]
  public class UsesCommonRules : BusinessBase<UsesCommonRules>
  {
    private static PropertyInfo<int> DataProperty = RegisterProperty<int>(c => c.Data, null, 1);
    public int Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    private static PropertyInfo<string> MinCheckProperty = RegisterProperty<string>(c => c.MinCheck, null, "123456");
    public string MinCheck
    {
      get { return GetProperty(MinCheckProperty); }
      set { SetProperty(MinCheckProperty, value); }
    }

    private static PropertyInfo<string> MaxCheckProperty = RegisterProperty<string>(c => c.MaxCheck);
    public string MaxCheck
    {
      get { return GetProperty(MaxCheckProperty); }
      set { SetProperty(MaxCheckProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MinValue<int>(DataProperty, 5));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxValue<int>(DataProperty, 15));

      BusinessRules.AddRule(new Csla.Rules.CommonRules.MinLength(MinCheckProperty, 5));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(MaxCheckProperty, 5));
    }
  }

  [Serializable]
  public class MinMaxNullableRules : BusinessBase<MinMaxNullableRules>
  {
    private static PropertyInfo<int?> DataNullableProperty = RegisterProperty<int?>(c => c.DataNullable);
    public int? DataNullable
    {
      get { return GetProperty(DataNullableProperty); }
      set { SetProperty(DataNullableProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new Csla.Rules.CommonRules.MinValue<int>(DataNullableProperty, 5));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxValue<int>(DataNullableProperty, 15));
    }
  }

  [Serializable]
  public class TwoPropertyRules : BusinessBase<TwoPropertyRules>
  {
    public static PropertyInfo<string> Value1Property = RegisterProperty<string>(c => c.Value1);
    public string Value1
    {
      get { return GetProperty(Value1Property); }
      set { SetProperty(Value1Property, value); }
    }

    public static PropertyInfo<string> Value2Property = RegisterProperty<string>(c => c.Value2);
    public string Value2
    {
      get { return GetProperty(Value2Property); }
      set { SetProperty(Value2Property, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new TwoProps(Value1Property, Value2Property));
      BusinessRules.AddRule(new TwoProps(Value2Property, Value1Property));
    }
  }

  public class TwoProps : Csla.Rules.BusinessRule
  {
    public Csla.Core.IPropertyInfo SecondaryProperty { get; set; }
    public TwoProps(Csla.Core.IPropertyInfo primaryProperty, Csla.Core.IPropertyInfo secondProperty)
      : base(primaryProperty)
    {
      SecondaryProperty = secondProperty;
      AffectedProperties.Add(SecondaryProperty);
      InputProperties = new List<Core.IPropertyInfo> { PrimaryProperty, SecondaryProperty };
    }

    protected override void Execute(Rules.IRuleContext context)
    {
      var v1 = (string)context.InputPropertyValues[PrimaryProperty];
      var v2 = (string)context.InputPropertyValues[SecondaryProperty];
      if (string.IsNullOrEmpty(v1) || string.IsNullOrEmpty(v2))
        context.AddErrorResult(string.Format("v1:{0}, v2:{1}", v1, v2));
    }
  }

  [Serializable]
  public class HasLazyField : BusinessBase<HasLazyField>
  {
    public static PropertyInfo<string> Value1Property = RegisterProperty<string>(nameof(Value1), RelationshipTypes.LazyLoad);
    public string Value1
    {
      get
      {
        return LazyGetProperty(Value1Property, () => string.Empty);
        //if (!FieldManager.FieldExists(Value1Property))
        //  SetProperty(Value1Property, string.Empty);
        //return GetProperty(Value1Property);
      }
      set { SetProperty(Value1Property, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new CheckLazyInputFieldExists(Value1Property));
    }

    public void CheckRules()
    {
      BusinessRules.CheckRules();
    }
  }

  [Serializable]
  public class DirtyAfterOutValueChangesProperty : BusinessBase<DirtyAfterOutValueChangesProperty>
  {
    public static PropertyInfo<string> Value1Property = RegisterProperty<string>(c => c.Value1);
    public string Value1
    {
      get { return GetProperty(Value1Property); }
      set { SetProperty(Value1Property, value); }
    }

    public DirtyAfterOutValueChangesProperty() : this("csla rocks")
    { }

    public DirtyAfterOutValueChangesProperty(string value)
    {
      using (BypassPropertyChecks)
      {
        Value1 = value;
      }
      MarkOld();
      MarkClean();
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new ToUpper(Value1Property));
    }

    private class ToUpper : Csla.Rules.BusinessRule
    {
      public ToUpper(IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        InputProperties = new List<IPropertyInfo>(){primaryProperty};
      }

      protected override void Execute(IRuleContext context)
      {
        var value = (string) context.InputPropertyValues[PrimaryProperty];
        context.AddOutValue(PrimaryProperty, value.ToUpperInvariant());
      }
    }


    public void CheckRules()
    {
      BusinessRules.CheckRules();
    }
  }


  public class CheckLazyInputFieldExists : Csla.Rules.BusinessRule
  {
    public CheckLazyInputFieldExists(Csla.Core.IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties = new List<Core.IPropertyInfo> { primaryProperty };
    }

    protected override void Execute(Rules.IRuleContext context)
    {
      if (context.InputPropertyValues.ContainsKey(PrimaryProperty))
      {
        context.AddErrorResult("PrimaryProperty has value.");
      }
      else
      {
        context.AddErrorResult("PrimaryProperty does not exist.");
      }


    }
  }


  [TestClass]
  public class RuleContextTests
  {
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddErrorResultThrowsErrorWhenMessageIsEmpty()
    {
      var root = new TwoPropertyRules();
      var rule = new TwoProps(TwoPropertyRules.Value1Property, TwoPropertyRules.Value2Property);

      var ctx = new Csla.Rules.RuleContext(null, rule, root,
                                           new Dictionary<Core.IPropertyInfo, object>
                                               {
                                                 {TwoPropertyRules.Value1Property, "a"},
                                                 {TwoPropertyRules.Value2Property, "b"}
                                               });
      ctx.AddErrorResult(string.Empty, false);
      Assert.Fail("Must throw exception.");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddErrorResultOnPropertyThrowsErrorWhenMessageIsEmpty()
    {
      var root = new TwoPropertyRules();
      var rule = new TwoProps(TwoPropertyRules.Value1Property, TwoPropertyRules.Value2Property);

      var ctx = new Csla.Rules.RuleContext(null, rule, root,
                                           new Dictionary<Core.IPropertyInfo, object>
                                               {
                                                 {TwoPropertyRules.Value1Property, "a"},
                                                 {TwoPropertyRules.Value2Property, "b"}
                                               });
      ctx.AddErrorResult(TwoPropertyRules.Value1Property, string.Empty);
      Assert.Fail("Must throw exception.");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddWarningResultThrowsErrorWhenMessageIsEmpty()
    {
      var root = new TwoPropertyRules();
      var rule = new TwoProps(TwoPropertyRules.Value1Property, TwoPropertyRules.Value2Property);

      var ctx = new Csla.Rules.RuleContext(null, rule, root,
                                           new Dictionary<Core.IPropertyInfo, object>
                                               {
                                                 {TwoPropertyRules.Value1Property, "a"},
                                                 {TwoPropertyRules.Value2Property, "b"}
                                               });
      ctx.AddWarningResult(string.Empty, false);
      Assert.Fail("Must throw exception.");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddWarningResultOnPropertyThrowsErrorWhenMessageIsEmpty()
    {
      var root = new TwoPropertyRules();
      var rule = new TwoProps(TwoPropertyRules.Value1Property, TwoPropertyRules.Value2Property);

      var ctx = new Csla.Rules.RuleContext(null, rule, root,
                                           new Dictionary<Core.IPropertyInfo, object>
                                               {
                                                 {TwoPropertyRules.Value1Property, "a"},
                                                 {TwoPropertyRules.Value2Property, "b"}
                                               });
      ctx.AddWarningResult(TwoPropertyRules.Value1Property, string.Empty);
      Assert.Fail("Must throw exception.");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddInformationResultThrowsErrorWhenMessageIsEmpty()
    {
      var root = new TwoPropertyRules();
      var rule = new TwoProps(TwoPropertyRules.Value1Property, TwoPropertyRules.Value2Property);

      var ctx = new Csla.Rules.RuleContext(null, rule, root,
                                           new Dictionary<Core.IPropertyInfo, object>
                                               {
                                                 {TwoPropertyRules.Value1Property, "a"},
                                                 {TwoPropertyRules.Value2Property, "b"}
                                               });
      ctx.AddInformationResult(string.Empty, false);
      Assert.Fail("Must throw exception.");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddInformationResultOnPropertyThrowsErrorWhenMessageIsEmpty()
    {
      var root = new TwoPropertyRules();
      var rule = new TwoProps(TwoPropertyRules.Value1Property, TwoPropertyRules.Value2Property);

      var ctx = new Csla.Rules.RuleContext(null, rule, root,
                                           new Dictionary<Core.IPropertyInfo, object>
                                               {
                                                 {TwoPropertyRules.Value1Property, "a"},
                                                 {TwoPropertyRules.Value2Property, "b"}
                                               });
      ctx.AddInformationResult(TwoPropertyRules.Value1Property, string.Empty);
      Assert.Fail("Must throw exception.");
    }


    [TestMethod]
    public void AddSuccessResultDoesNotFail()
    {
      var root = new TwoPropertyRules();
      var rule = new TwoProps(TwoPropertyRules.Value1Property, TwoPropertyRules.Value2Property);

      var ctx = new Csla.Rules.RuleContext(null, rule, root,
                                           new Dictionary<Core.IPropertyInfo, object>
                                               {
                                                 {TwoPropertyRules.Value1Property, "a"},
                                                 {TwoPropertyRules.Value2Property, "b"}
                                               });
      ctx.AddSuccessResult(false);
      Assert.IsTrue(true, "Must not fail.");
    }
  }
}