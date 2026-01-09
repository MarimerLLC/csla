using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTracker.DalEfCore
{
  public class PTrackerContext : DbContext
  {
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Role> Roles { get; set; }

    public PTrackerContext(DbContextOptions<PTrackerContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Assignment>()
        .HasKey(o => new { o.ProjectId, o.ResourceId })
        .HasName("PK_Assignments");

      // Configure concurrency tokens for SQLite (using byte[] with manual versioning)
      modelBuilder.Entity<Assignment>()
        .Property(e => e.LastChanged)
        .IsConcurrencyToken();

      modelBuilder.Entity<Project>()
        .Property(e => e.LastChanged)
        .IsConcurrencyToken();

      modelBuilder.Entity<Resource>()
        .Property(e => e.LastChanged)
        .IsConcurrencyToken();

      modelBuilder.Entity<Role>()
        .Property(e => e.LastChanged)
        .IsConcurrencyToken();
    }

    public override int SaveChanges()
    {
      UpdateLastChanged();
      return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
      UpdateLastChanged();
      return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateLastChanged()
    {
      var entries = ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

      foreach (var entry in entries)
      {
        if (entry.Entity is IHasLastChanged entity)
        {
          entity.LastChanged = GenerateTimestamp();
        }
      }
    }

    private static long _timestampCounter = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static byte[] GenerateTimestamp()
    {
      var stamp = System.Threading.Interlocked.Increment(ref _timestampCounter);
      return BitConverter.GetBytes(stamp);
    }
  }

  public interface IHasLastChanged
  {
    byte[] LastChanged { get; set; }
  }

  public class Assignment : IHasLastChanged
  {
    public int ProjectId { get; set; }
    public int ResourceId { get; set; }
    public DateTime Assigned { get; set; }
    public int RoleId { get; set; }
    public byte[] LastChanged { get; set; } = Array.Empty<byte>();
  }

  public class Project : IHasLastChanged
  {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? Started { get; set; }
    public DateTime? Ended { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<Assignment> Assignments { get; } = new List<Assignment>();
    public byte[] LastChanged { get; set; } = Array.Empty<byte>();
  }

  public class Resource : IHasLastChanged
  {
    [Key]
    public int Id { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public List<Assignment> Assignments { get; } = new List<Assignment>();
    public byte[] LastChanged { get; set; } = Array.Empty<byte>();
  }

  public class Role : IHasLastChanged
  {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public byte[] LastChanged { get; set; } = Array.Empty<byte>();
  }
}