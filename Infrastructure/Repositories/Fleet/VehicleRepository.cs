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

            
            public async Task<Vehicle?> GetVehicleWithDetailsAsync(Guid id)
            {
                return await _dbSet
                    .Include(v => v.MaintenanceRecords)
                    .Include(v => v.CurrentDriver)
                    .FirstOrDefaultAsync(v => v.Id == id);
            }
        }
    
}