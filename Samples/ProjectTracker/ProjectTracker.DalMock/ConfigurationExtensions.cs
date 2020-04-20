using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.DalMock;
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
    /// use Entity Framework
    /// </summary>
    /// <param name="services"></param>
    public static void AddDalMock(this IServiceCollection services)
    {
      services.AddTransient<IAssignmentDal, AssignmentDal>();
      services.AddTransient<IDashboardDal, DashboardDal>();
      services.AddTransient<IProjectDal, ProjectDal>();
      services.AddTransient<IResourceDal, ResourceDal>();
      services.AddTransient<IRoleDal, RoleDal>();
      services.AddTransient<IUserDal, UserDal>();
    }
  }
}
