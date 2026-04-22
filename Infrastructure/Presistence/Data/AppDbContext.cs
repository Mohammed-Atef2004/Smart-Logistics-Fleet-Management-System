using Domain.Drivers;
using Domain.InventoryItems;
using Domain.SharedKernel;
using Domain.Shifts;
using Domain.Shipments;
using Domain.Users;
using Domain.Vehicles;
using Domain.Warehouse;
using Infrastructure.Presistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Presistence.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<DomainEvent>();

            // Apply all configurations automatically
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            
        }
    }
}
