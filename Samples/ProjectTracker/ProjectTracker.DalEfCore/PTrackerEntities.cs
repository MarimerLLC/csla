using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.DalEfCore
{
  public class PTrackerContext : DbContext
  {
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Assignment>()
        .HasKey(o => new { o.ProjectId, o.ResourceId })
        .HasName("PK_Assignments");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=E:\\src\\rdl\\csla\\Samples\\ProjectTracker\\ProjectTracker.AppServerCore\\PTracker.mdf;Integrated Security=True;Connect Timeout=30");
  }

  public class Assignment
  {
    public int ProjectId { get; set; }
    public int ResourceId { get; set; }
    public DateTime Assigned { get; set; }
    public int RoleId { get; set; }
    [Timestamp]
    public byte[] LastChanged { get; set; }
  }

  public class Project
  {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime? Started { get; set; }
    public DateTime? Ended { get; set; }
    public string Description { get; set; }
    public List<Assignment> Assignments { get; } = new List<Assignment>();
    [Timestamp]
    public byte[] LastChanged { get; set; }
  }

  public class Resource
  {
    [Key]
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public List<Assignment> Assignments { get; } = new List<Assignment>();
    [Timestamp]
    public byte[] LastChanged { get; set; }
  }

  public class Role
  {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    [Timestamp]
    public byte[] LastChanged { get; set; }
  }
}