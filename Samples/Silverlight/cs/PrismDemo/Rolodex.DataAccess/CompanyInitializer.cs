using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Rolodex.Data;

namespace Rolodex.DataAccess
{
    public class CompanyInitializer : DropCreateDatabaseIfModelChanges<CompanyContext>
    {
        protected override void Seed(CompanyContext context)
        {
            base.Seed(context);
            context.Users.Add(new User() { UserLogin = "admin", UserPassword = "admin", UserRole = "ReadWrite"});
            context.Users.Add(new User() { UserLogin = "ReadOnlyUser", UserPassword = "readonly", UserRole = "ReadOnly" });
            context.SaveChanges();
        }
    }
}
