using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Csla.Data.EF7.Test
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=(LocalDB)\v11.0;Database=CslaEF7Test;Trusted_Connection=True;");
        }

        public DbSet<Table2> Table2 { get; set; }
    }
}
