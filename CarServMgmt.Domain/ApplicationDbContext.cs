using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using CarServMgmt.Domain;

namespace CarServMgmt.Domain;

public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Mechanic> Mechanics { get; set; }
    public DbSet<Receptionist> Receptionists { get; set; }
    public DbSet<ServiceRecord> ServiceRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Cars)
            .WithOne(c => c.Customer)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Car>()
            .HasMany(c => c.ServiceRecords)
            .WithOne(sr => sr.Car)
            .HasForeignKey(sr => sr.CarId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Mechanic>()
            .HasMany(m => m.ServiceRecords)
            .WithOne(sr => sr.Mechanic)
            .HasForeignKey(sr => sr.MechanicId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "John Doe", Email = "customer1@carservice.com" },
            new Customer { Id = 2, Name = "Jane Smith", Email = "customer2@carservice.com" }
        );
        
        modelBuilder.Entity<Mechanic>().HasData(
            new Mechanic { Id = 1, Name = "Mike Mechanic", Email = "mechanic1@carservice.com" },
            new Mechanic { Id = 2, Name = "Sara Spanner", Email = "mechanic2@carservice.com" }
        );
        
        modelBuilder.Entity<Receptionist>().HasData(
            new Receptionist { Id = 1, Name = "Alice Reception", Email = "reception1@carservice.com" },
            new Receptionist { Id = 2, Name = "Bob FrontDesk", Email = "reception2@carservice.com" }
        );
        
        modelBuilder.Entity<Car>().HasData(
            new Car { Id = 1, RegistrationNumber = "ABC123", CustomerId = 1 },
            new Car { Id = 2, RegistrationNumber = "XYZ789", CustomerId = 2 }
        );
        
        modelBuilder.Entity<ServiceRecord>().HasData(
            new ServiceRecord { Id = 1, CarId = 1, MechanicId = 1, Description = "Oil Change", Hours = 1.5, IsComplete = true, DateBroughtIn = new DateTime(2024, 7, 1), DateCompleted = new DateTime(2024, 7, 2) },
            new ServiceRecord { Id = 2, CarId = 2, MechanicId = 2, Description = "Brake Replacement", Hours = 2, IsComplete = false, DateBroughtIn = new DateTime(2024, 7, 3), DateCompleted = null }
        );
    }
} 