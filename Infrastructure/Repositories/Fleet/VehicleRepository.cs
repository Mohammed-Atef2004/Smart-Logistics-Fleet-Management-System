using Domain.Fleet;
using Domain.Interfaces.Repositories.Fleet;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories;
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
    }
}