using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

namespace Csla.Data.EF4.Test
{
    [Table("Table2")]
    public class Table2
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SmallColumns { get; set; }
    }

    public partial class DataPortalDbContext : DbContext
    {
        public DataPortalDbContext()
        {
        }

        public DataPortalDbContext(string connection)
            : base(connection)
        {
        }

        public DataPortalDbContext(DbCompiledModel model)
            : base(model)
        {
        }

        public DataPortalDbContext(string database, DbCompiledModel model)
            : base(database, model)
        {
        }

        public DataPortalDbContext(ObjectContext context, bool contextOwnsConnection)
            : base(context, contextOwnsConnection)
        {
        }

        public DataPortalDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
        }

        public DataPortalDbContext(DbConnection connection, DbCompiledModel model, bool contextOwnsConnection)
            : base(connection, model, contextOwnsConnection)
        {
        }

        public DbSet<Table2> Table2 { get; set; }
    }
}
