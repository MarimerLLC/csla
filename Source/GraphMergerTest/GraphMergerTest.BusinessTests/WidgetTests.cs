using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Csla.Rules;
using GraphMergerTest.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphMergerTest.BusinessTests
{
    [TestClass]
    public class WidgetTests
        : TestBase
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestBaseClassInitialize();
        }

        [TestMethod]
        public async Task UpdateWithMergeWidgetTest()
        {
            await UpdateWidgetTest(true);
        }

        [TestMethod]
        public async Task UpdateWithoutMergeWidgetTest()
        {
            await UpdateWidgetTest(false);
        }

    public async Task UpdateWidgetTest(bool merge)
    {
      var instance = await InsertWidgetAsync();

      instance = await WidgetFactory.GetAsync(instance.Id);

      instance.ChildItems.AddNew(new[] { Guid.NewGuid() });

      var allChildItemIds = instance.ChildItems.Select(d => d.ChildItemId).ToList();

      Assert.IsTrue(instance.IsValid, BusinessRules.GetAllBrokenRules(instance).ToString());

      foreach (var childItem in instance.ChildItems)
        Console.WriteLine(childItem.ToString());

      if (merge)
        await instance.SaveAndMergeAsync();
      else
        instance = await instance.SaveAsync();

      foreach (var childItem in instance.ChildItems)
        Console.WriteLine(childItem.ToString());

      foreach (var childitemId in allChildItemIds)
        Assert.IsNotNull(instance.ChildItems.GetChildItem(childitemId));
    }

        public static async Task<Widget> NewWidgetAsync()
        {
            var instance = await WidgetFactory.NewWidgetAsync();

            AssignValues(instance);

            return instance;
        }

        public static async Task<Widget> InsertWidgetAsync()
        {
            var instance = await NewWidgetAsync();

            instance     = instance.Save();

            return instance;
        }

        public static void AssignValues(Widget instance)
        {
            var childitemIds = new List<Guid>();

            for (int i = 0; i < 4; i++)
                childitemIds.Add(Guid.NewGuid());

            instance.ChildItems.AddNew(childitemIds);
        }
    }
}
