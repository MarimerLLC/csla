using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.DalEfCore;
using ProjectTracker.Dal;

namespace ProjectTracker.Configuration
{
  /// <summary>
  /// Configuration extension methods
  /// </summary>
  public static class ConfigurationExtensions
  {
    /// <summary>
    /// Add the services for ProjectTracker.Dal that
    /// use Entity Framework with SQLite
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString">SQLite connection string (defaults to Data Source=PTracker.db)</param>
    public static void AddDalEfCore(this IServiceCollection services, string connectionString = "Data Source=PTracker.db")
    {
      services.AddDbContext<PTrackerContext>(options =>
        options.UseSqlite(connectionString));
      services.AddTransient<IAssignmentDal, AssignmentDal>();
      services.AddTransient<IDashboardDal, DashboardDal>();
      services.AddTransient<IProjectDal, ProjectDal>();
      services.AddTransient<IResourceDal, ResourceDal>();
      services.AddTransient<IRoleDal, RoleDal>();
      services.AddTransient<IUserDal, UserDal>();
    }
  }
}
