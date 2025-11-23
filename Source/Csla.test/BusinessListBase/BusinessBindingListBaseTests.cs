//-----------------------------------------------------------------------
// <copyright file="BusinessBindingListBaseTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Serialization;
using Csla.TestHelpers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.BusinessListBase
{
  [TestClass]
  public class BusinessBindingListBaseTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public async Task Parent_LocationTransferWithDeletedItemsMustSetParentOnDeletedItems()
    {
      var root = CreateRoot();

      for (int i = 0; i < 5; i++)
      {
        root.Children.AddNew();
      }

      root = await root.SaveAsync();
      root.Children.Clear();

      var transferredGraph = SimulateLocationTransfer(root);

      using (new AssertionScope())
      {
        transferredGraph.Children.Parent.Should().BeSameAs(transferredGraph);
        transferredGraph.Children.DeletedItems.Should().AllSatisfy(c => c.Parent.Should().BeSameAs(transferredGraph.Children));
      }


      static Basic.Root SimulateLocationTransfer(Basic.Root original)
      {
        var serializer = _testDIContext.ServiceProvider.GetRequiredService<ISerializationFormatter>();
        return (Basic.Root)serializer.Deserialize(serializer.Serialize(original));
      }
    }

    private static Basic.Root CreateRoot()
    {
      var dataPortal = _testDIContext.CreateDataPortal<Basic.Root>();

      return dataPortal.Create(new Basic.Root.Criteria("Random"));
    }
  }
}