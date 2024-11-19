using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DreamTeamOptimizer.ConsoleApp.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Preference> Preferences { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Satisfaction> Satisfactions { get; set; }
    public DbSet<Hackathon> Hackathons { get; set; }

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Host=localhost;Port=25432;Database=hackathon;Username=admin;Password=password";
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Employee
        modelBuilder.Entity<Employee>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<Employee>()
            .Property(e => e.Grade)
            .HasConversion(new EnumToStringConverter<Grade>());
        modelBuilder.Entity<Employee>()
            .HasMany(e => e.Hackathons)
            .WithMany(p => p.Employees)
            .UsingEntity<HackathonEmployee>(
                j => j.HasOne(he => he.Hackathon)
                    .WithMany()
                    .HasForeignKey(he => he.HackathonId),
                j => j.HasOne(he => he.Employee)
                    .WithMany()
                    .HasForeignKey(he => he.EmployeeId)
            );

        // Hackathon
        modelBuilder.Entity<Hackathon>()
            .HasKey(h => h.Id);
        modelBuilder.Entity<Hackathon>()
            .Property(e => e.Status)
            .HasConversion(new EnumToStringConverter<HackathonStatus>());
        modelBuilder.Entity<Hackathon>()
            .HasMany(h => h.Employees)
            .WithMany(p => p.Hackathons)
            .UsingEntity<HackathonEmployee>(
                j => j.HasOne(he => he.Employee)
                    .WithMany()
                    .HasForeignKey(he => he.EmployeeId),
                j => j.HasOne(he => he.Hackathon)
                    .WithMany()
                    .HasForeignKey(he => he.HackathonId)
            );
        modelBuilder.Entity<Hackathon>()
            .HasMany(e => e.Preferences)
            .WithOne(p => p.Hackathon);
        modelBuilder.Entity<Hackathon>()
            .HasMany(e => e.Preferences)
            .WithOne(p => p.Hackathon);
        modelBuilder.Entity<Hackathon>()
            .HasMany(e => e.Preferences)
            .WithOne(p => p.Hackathon);

        // Preference
        modelBuilder.Entity<Preference>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Preference>()
            .HasOne(p => p.Employee)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Preference>()
            .HasOne(p => p.DesiredEmployee)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        // Satisfaction
        modelBuilder.Entity<Satisfaction>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Satisfaction>()
            .HasOne(p => p.Employee)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Satisfaction>()
            .HasOne(p => p.Hackathon)
            .WithMany(p => p.Satisfactions)
            .OnDelete(DeleteBehavior.Cascade);

        // Team
        modelBuilder.Entity<Team>()
            .HasKey(t => t.Id);
        modelBuilder.Entity<Team>()
            .HasOne(t => t.Junior)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Team>()
            .HasOne(t => t.TeamLead)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Team>()
            .HasOne(t => t.Hackathon)
            .WithMany(h => h.Teams)
            .OnDelete(DeleteBehavior.Cascade);

        // Team
        modelBuilder.Entity<Satisfaction>()
            .HasKey(t => t.Id);
        modelBuilder.Entity<Satisfaction>()
            .HasOne(t => t.Employee)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Satisfaction>()
            .HasOne(t => t.Hackathon)
            .WithMany(h => h.Satisfactions)
            .OnDelete(DeleteBehavior.Cascade);
    }
}