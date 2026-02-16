using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectTracker.DalEfCore
{
  /// <summary>
  /// Initializes and seeds the SQLite database
  /// </summary>
  public static class PTrackerDatabaseInitializer
  {
    /// <summary>
    /// Ensures the database is created and seeded with initial data
    /// </summary>
    /// <param name="serviceProvider">The service provider to get the DbContext from</param>
    public static void Initialize(IServiceProvider serviceProvider)
    {
      using var scope = serviceProvider.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<PTrackerContext>();

      // Create the database if it doesn't exist
      context.Database.EnsureCreated();

      // Seed data only if the database is empty
      if (!context.Roles.Any())
      {
        SeedData(context);
      }
    }

    /// <summary>
    /// Seeds the database with initial data matching MockDb
    /// </summary>
    private static void SeedData(PTrackerContext context)
    {
      // Seed Roles
      var roles = new List<Role>
      {
        new Role { Id = 1, Name = "Project manager", LastChanged = PTrackerContext.GenerateTimestamp() },
        new Role { Id = 2, Name = "Developer", LastChanged = PTrackerContext.GenerateTimestamp() },
        new Role { Id = 3, Name = "QA", LastChanged = PTrackerContext.GenerateTimestamp() },
        new Role { Id = 4, Name = "Sponsor", LastChanged = PTrackerContext.GenerateTimestamp() }
      };
      context.Roles.AddRange(roles);
      context.SaveChanges();

      // Seed Resources
      var resources = new List<Resource>
      {
        new Resource { Id = 1, FirstName = "Rocky", LastName = "Lhotka", LastChanged = PTrackerContext.GenerateTimestamp() },
        new Resource { Id = 2, FirstName = "Meghan", LastName = "Lien", LastChanged = PTrackerContext.GenerateTimestamp() },
        new Resource { Id = 3, FirstName = "Jason", LastName = "Bock", LastChanged = PTrackerContext.GenerateTimestamp() }
      };
      context.Resources.AddRange(resources);
      context.SaveChanges();

      // Seed Projects
      var projects = new List<Project>
      {
        new Project
        {
          Id = 1,
          Name = "Update ProjectTracker",
          Description = "Update ProjectTracker for CSLA 10",
          Started = new DateTime(2024, 2, 10),
          LastChanged = PTrackerContext.GenerateTimestamp()
        },
        new Project
        {
          Id = 2,
          Name = "CSLA Documentation",
          Description = "Update CSLA documentation and samples",
          Started = new DateTime(2024, 1, 15),
          LastChanged = PTrackerContext.GenerateTimestamp()
        }
      };
      context.Projects.AddRange(projects);
      context.SaveChanges();

      // Seed Assignments
      var assignments = new List<Assignment>
      {
        new Assignment
        {
          ProjectId = 1,
          ResourceId = 1,
          Assigned = new DateTime(2024, 2, 10),
          RoleId = 2,
          LastChanged = PTrackerContext.GenerateTimestamp()
        },
        new Assignment
        {
          ProjectId = 1,
          ResourceId = 2,
          Assigned = new DateTime(2024, 2, 15),
          RoleId = 3,
          LastChanged = PTrackerContext.GenerateTimestamp()
        },
        new Assignment
        {
          ProjectId = 2,
          ResourceId = 1,
          Assigned = new DateTime(2024, 1, 15),
          RoleId = 1,
          LastChanged = PTrackerContext.GenerateTimestamp()
        },
        new Assignment
        {
          ProjectId = 2,
          ResourceId = 3,
          Assigned = new DateTime(2024, 1, 20),
          RoleId = 2,
          LastChanged = PTrackerContext.GenerateTimestamp()
        }
      };
      context.Assignments.AddRange(assignments);
      context.SaveChanges();
    }

    /// <summary>
    /// Resets and reseeds the database (useful for testing)
    /// </summary>
    /// <param name="serviceProvider">The service provider to get the DbContext from</param>
    public static void ResetDatabase(IServiceProvider serviceProvider)
    {
      using var scope = serviceProvider.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<PTrackerContext>();

      context.Database.EnsureDeleted();
      context.Database.EnsureCreated();
      SeedData(context);
    }
  }
}
