using System;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Csla.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Data.EF5.Test
{
    [TestClass]
    public class DbContextTests
    {
        private const string TestDBConnection = "Csla.Test.Properties.Settings.DataPortalTestDatabaseConnectionString";
        private const string InvalidTestDBConnection = "Csla.Test.Properties.Settings.DataPortalTestDatabaseConnectionStringXXXXXXX";

        private const string ConnectionWithMissingDB = "DataPortalTestDatabaseConnectionString_with_invalid_DB_value";
        private const string EntityConnectionWithMissingDB = "DataPortalTestDatabaseEntities_with_invalid_DB_value";

        [ClassInitialize]
        public static void InitDbContextDatabase(TestContext context)
        {
            using (var dbContextManager = DbContextManager<DataPortalDbContext>.GetManager())
            {
                dbContextManager.DbContext.Database.CreateIfNotExists();
                dbContextManager.DbContext.Database.Initialize(true);

                dbContextManager.DbContext.Table2.Add(new Table2()
                {
                    FirstName = "Rocky",
                    Id = 1,
                    LastName = "Lhotka",
                    SmallColumns = "Test"
                });
                dbContextManager.DbContext.SaveChanges();
            }
        }

#if DEBUG
        [TestMethod]
        [ExpectedException(typeof(EntityException))]
        public void ConnectionSetting_with_Invalid_DB_Throws_ConfigurationErrorsException_for_DbContextDataContext()
        {
            var conn = ConfigurationManager.ConnectionStrings[EntityConnectionWithMissingDB].ConnectionString;
            using (var context = new DataPortalTestDatabaseEntities(conn))
            {
                using (
                  var dbContextManager = DbContextManager<DataPortalDbContext>.GetManager(context))
                {
                    Assert.IsNotNull(dbContextManager);

                    // Make sure the context is set in DbContext
                    Assert.AreSame(context, ((IObjectContextAdapter)dbContextManager.DbContext).ObjectContext);
                    //Throws EntityException
                    var table = (from p in context.Table2
                                 select p).ToList();
                }
            }
        }
#endif

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
    }
}
