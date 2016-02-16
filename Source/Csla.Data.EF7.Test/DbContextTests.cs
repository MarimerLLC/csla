using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Data.EF7.Test
{
    [TestClass]
    public class DbContextTests
    {
   
        [ClassInitialize]
        public static void InitDbContextDatabase(TestContext context)
        {
            using (var dbContextManager = DbContextManager<DataPortalDbContext>.GetManager())
            {
                dbContextManager.DbContext.Database.EnsureCreated();              

                dbContextManager.DbContext.Table2.Add(new Table2()
                {
                    FirstName = "Rocky",
                    LastName = "Lhotka",
                    SmallColumns = "Test"
                });
                dbContextManager.DbContext.SaveChanges();
            }
        }

#if DEBUG
      
        [TestMethod]
        public void Table2_retreived_through_DbContextDataContext_has_records()
        {
            using (var dbContextManager = DbContextManager<DataPortalDbContext>.GetManager())
            {
                Assert.IsNotNull(dbContextManager);

                var query = dbContextManager.DbContext.Table2;
                Assert.IsTrue(query.Any(), "Data in table is missing");
            }
        }
#endif
  }
}
