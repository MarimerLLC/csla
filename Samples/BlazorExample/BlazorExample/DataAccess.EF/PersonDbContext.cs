using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.EF
{
    public class PersonDbContext : DbContext
    {
        public DbSet<PersonEntity> Persons { get; set; }

        public PersonDbContext(DbContextOptions<PersonDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }

    }
}
