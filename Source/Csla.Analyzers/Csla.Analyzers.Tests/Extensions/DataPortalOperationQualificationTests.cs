using Csla.Analyzers.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Analyzers.Tests.Extensions
{
  [TestClass]
  public sealed class DataPortalOperationQualificationTests
  {
    [TestMethod]
    public void Create()
    {
      var qualification = new DataPortalOperationQualification();
      Assert.IsFalse(qualification.ByAttribute, nameof(qualification.ByAttribute));
      Assert.IsFalse(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
    }

    [TestMethod]
    public void CreateWithByAttributeArgumentAsTrue()
    {
      var qualification = new DataPortalOperationQualification(false, true);
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
      Assert.IsFalse(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
    }

    [TestMethod]
    public void CreateWithByNamingConventionArgumentAsTrue()
    {
      var qualification = new DataPortalOperationQualification(true, false);
      Assert.IsFalse(qualification.ByAttribute, nameof(qualification.ByAttribute));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
    }

    [TestMethod]
    public void CompareWhenBothFlagsAreFalse() => 
      Assert.IsFalse(new DataPortalOperationQualification());

    [TestMethod]
    public void CompareWhenByAttributeFlagIsTrue() =>
      Assert.IsTrue(new DataPortalOperationQualification(false, true));

    [TestMethod]
    public void CompareWhenByNamingConventionFlagIsTrue() =>
      Assert.IsTrue(new DataPortalOperationQualification(true, false));

    [TestMethod]
    public void CombineFalseAndFalseWithFalseAndFalse()
    {
      var qualification = new DataPortalOperationQualification(false, false).Combine(
        new DataPortalOperationQualification(false, false));
      Assert.IsFalse(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsFalse(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineFalseAndFalseWithFalseAndTrue()
    {
      var qualification = new DataPortalOperationQualification(false, false).Combine(
        new DataPortalOperationQualification(false, true));
      Assert.IsFalse(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineFalseAndFalseWithTrueAndFalse()
    {
      var qualification = new DataPortalOperationQualification(false, false).Combine(
        new DataPortalOperationQualification(true, false));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsFalse(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineFalseAndFalseWithTrueAndTrue()
    {
      var qualification = new DataPortalOperationQualification(false, false).Combine(
        new DataPortalOperationQualification(true, true));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineFalseAndTrueWithFalseAndFalse()
    {
      var qualification = new DataPortalOperationQualification(false, true).Combine(
        new DataPortalOperationQualification(false, false));
      Assert.IsFalse(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineFalseAndTrueWithFalseAndTrue()
    {
      var qualification = new DataPortalOperationQualification(false, true).Combine(
        new DataPortalOperationQualification(false, true));
      Assert.IsFalse(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineFalseAndTrueWithTrueAndFalse()
    {
      var qualification = new DataPortalOperationQualification(false, true).Combine(
        new DataPortalOperationQualification(true, false));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineFalseAndTrueWithTrueAndTrue()
    {
      var qualification = new DataPortalOperationQualification(false, true).Combine(
        new DataPortalOperationQualification(true, true));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineTrueAndFalseWithFalseAndFalse()
    {
      var qualification = new DataPortalOperationQualification(true, false).Combine(
        new DataPortalOperationQualification(false, false));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsFalse(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineTrueAndFalseWithFalseAndTrue()
    {
      var qualification = new DataPortalOperationQualification(true, false).Combine(
        new DataPortalOperationQualification(false, true));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineTrueAndFalseWithTrueAndFalse()
    {
      var qualification = new DataPortalOperationQualification(true, false).Combine(
        new DataPortalOperationQualification(true, false));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsFalse(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineTrueAndFalseWithTrueAndTrue()
    {
      var qualification = new DataPortalOperationQualification(true, false).Combine(
        new DataPortalOperationQualification(true, true));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineTrueAndTrueWithFalseAndFalse()
    {
      var qualification = new DataPortalOperationQualification(true, true).Combine(
        new DataPortalOperationQualification(false, false));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineTrueAndTrueWithFalseAndTrue()
    {
      var qualification = new DataPortalOperationQualification(true, true).Combine(
        new DataPortalOperationQualification(false, true));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineTrueAndTrueWithTrueAndFalse()
    {
      var qualification = new DataPortalOperationQualification(true, true).Combine(
        new DataPortalOperationQualification(true, false));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }

    [TestMethod]
    public void CombineTrueAndTrueWithTrueAndTrue()
    {
      var qualification = new DataPortalOperationQualification(true, true).Combine(
        new DataPortalOperationQualification(true, true));
      Assert.IsTrue(qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.IsTrue(qualification.ByAttribute, nameof(qualification.ByAttribute));
    }
  }
}