using Domain.Fleet.Entities;
using Domain.Fleet.Interfaces;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Fleet
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void Update(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
        }

        public async Task<Vehicle?> GetVehicleWithMaintenanceAsync(Guid id)
        {
            return await _context.Vehicles
                .Include(v => v.MaintenanceRecords)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
        public async Task<List<Vehicle>> GetAllVehiclesWithMaintenanceAsync()
        {
                 return await _context.Vehicles
                .Include(v => v.MaintenanceRecords)
                .ToListAsync();
        }
    }
}