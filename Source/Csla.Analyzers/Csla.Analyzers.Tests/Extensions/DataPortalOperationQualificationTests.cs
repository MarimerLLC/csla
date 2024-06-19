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
    public void DeconstructWhenFlagsAreFalseAndFalse()
    {
      var (byNamingConvention, byAttribute) = new DataPortalOperationQualification(false, false);
      Assert.IsFalse(byNamingConvention, nameof(byNamingConvention));
      Assert.IsFalse(byAttribute, nameof(byAttribute));
    }

    [TestMethod]
    public void DeconstructWhenFlagsAreFalseAndTrue()
    {
      var (byNamingConvention, byAttribute) = new DataPortalOperationQualification(false, true);
      Assert.IsFalse(byNamingConvention, nameof(byNamingConvention));
      Assert.IsTrue(byAttribute, nameof(byAttribute));
    }

    [TestMethod]
    public void DeconstructWhenFlagsAreTrueAndFalse()
    {
      var (byNamingConvention, byAttribute) = new DataPortalOperationQualification(true, false);
      Assert.IsTrue(byNamingConvention, nameof(byNamingConvention));
      Assert.IsFalse(byAttribute, nameof(byAttribute));
    }

    [TestMethod]
    public void DeconstructWhenFlagsAreTrueAndTrue()
    {
      var (byNamingConvention, byAttribute) = new DataPortalOperationQualification(true, true);
      Assert.IsTrue(byNamingConvention, nameof(byNamingConvention));
      Assert.IsTrue(byAttribute, nameof(byAttribute));
    }

    [DataRow(false, false, false, false, false, false)]
    [DataRow(false, false, false, true, false, true)]
    [DataRow(false, false, true, false, true, false)]
    [DataRow(false, false, true, true, true, true)]
    [DataRow(false, true, false, false, false, true)]
    [DataRow(false, true, false, true, false, true)]
    [DataRow(false, true, true, false, true, true)]
    [DataRow(false, true, true, true, true, true)]
    [DataRow(true, false, false, false, true, false)]
    [DataRow(true, false, false, true, true, true)]
    [DataRow(true, false, true, false, true, false)]
    [DataRow(true, false, true, true, true, true)]
    [DataRow(true, true, false, false, true, true)]
    [DataRow(true, true, false, true, true, true)]
    [DataRow(true, true, true, false, true, true)]
    [DataRow(true, true, true, true, true, true)]
    [DataTestMethod]
    public void Combine(bool firstByNamingConvention, bool firstByAttribute, bool secondByNamingConvention, bool secondByAttribute,
      bool expectedByNamingConvention, bool expectedByAttribute)
    {
      var qualification = new DataPortalOperationQualification(firstByNamingConvention, firstByAttribute).Combine(
        new DataPortalOperationQualification(secondByNamingConvention, secondByAttribute));
      Assert.AreEqual(expectedByNamingConvention, qualification.ByNamingConvention, nameof(qualification.ByNamingConvention));
      Assert.AreEqual(expectedByAttribute, qualification.ByAttribute, nameof(qualification.ByAttribute));
    }
  }
}