//-----------------------------------------------------------------------
// <copyright file="ReadWriteAuthorizationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Windows;

[TestClass]
public class BindingSourceHelperTests
{
    [TestMethod]
    [ExpectedException(typeof(ApplicationException))]
    public void InitializeBindingSourceTree_NullRootSource_ThrowsException()
    {
        // Arrange
        using IContainer container = new Container();

        // Act
        BindingSourceHelper.InitializeBindingSourceTree(container, null);
    }

    [TestMethod]
    public void InitializeBindingSourceTree_ValidRootSource_ReturnsRootNode()
    {
        // Arrange
        using var container = new Container();
        using var rootSource = new BindingSource();
        container.Add(rootSource);

        // Act
        var result = BindingSourceHelper.InitializeBindingSourceTree(container, rootSource);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(rootSource, result.Source);
    }

    [TestMethod]
    public void InitializeBindingSourceTree_WithChildSources_SetsUpTreeCorrectly()
    {
        // Arrange
        using var container = new Container();
        using var rootSource = new BindingSource();
        using var childSource = new BindingSource
        {
            DataSource = rootSource
        };
        container.Add(rootSource);
        container.Add(childSource);

        // Act
        var result = BindingSourceHelper.InitializeBindingSourceTree(container, rootSource);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Children.Count);
        Assert.AreEqual(childSource, result.Children[0].Source);
    }
}