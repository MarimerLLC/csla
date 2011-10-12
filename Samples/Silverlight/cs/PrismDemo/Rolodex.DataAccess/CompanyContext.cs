using System.Data.Entity;
using Rolodex.Data;

namespace Rolodex.DataAccess
{
    public class CompanyContext : DbContext
    {
        public CompanyContext(string connectionString)
            : base(connectionString)
        {

        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<EmployeeStatus> EmployeeStatuses { get; set; }
        public DbSet<Emlpoyee> Emlpoyees { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
