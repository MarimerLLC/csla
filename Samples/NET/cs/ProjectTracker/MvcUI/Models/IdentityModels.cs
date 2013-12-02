using Microsoft.AspNet.Identity.EntityFramework;

namespace MvcUI.Models
{
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUser : IdentityUser
  {
  }

  public class PTApplicationUser : IdentityUser, System.Security.Principal.IIdentity
  {
    private System.Security.Principal.IIdentity _identity;

    public PTApplicationUser()
    { }

    public PTApplicationUser(ProjectTracker.Library.Security.PTIdentity identity)
    {
      _identity = identity;
      this.UserName = Name;
      this.Id = Name;
      foreach (var item in identity.Roles)
      {
        this.Roles.Add(new IdentityUserRole { UserId = this.Id, RoleId = item, Role = new IdentityRole(item), User = this });
      }
    }

    public string AuthenticationType
    {
      get { return _identity.AuthenticationType; }
    }

    public bool IsAuthenticated
    {
      get { return _identity.IsAuthenticated; }
    }

    public string Name
    {
      get { return _identity.Name; }
    }
  }

  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
    public ApplicationDbContext()
      : base("DefaultConnection")
    {
    }
  }
}