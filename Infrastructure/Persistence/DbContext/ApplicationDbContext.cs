using Domain.Billing.Entities;
using Domain.Common;
using Domain.Fleet.Entities;
using Domain.Notifications.Entities;
using Domain.Shipment.Entities;
using Domain.Users.Entities;
using Domain.Warehouse;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options) { }

        // Fleet
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }

        public DbSet<Route> Routes { get; set; }
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

        // Shipment
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<TrackingUpdate> TrackingUpdates { get; set; }

        // Warehouse
        public DbSet<InventoryItem> InventoryItems { get; set; }

        // Billing
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<InsuranceClaim> InsuranceClaims { get; set; }

        // Notifications
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        // Users
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
            // Ignore abstract base classes
            modelBuilder.Ignore<DomainEvent>();

            base.OnModelCreating(modelBuilder);
        
    }
    }
}
