using Infrastructure.Presistence.Data;

namespace Infrastructure.Repositories.Vehicle
{
    using Domain.Interfaces.Repositories;
    using Domain.Vehicles;
    using Domain.Vehicles.Enums;
    using Domain.Vehicles.ValueObjects;
    using global::Infrastructure.Repositories.Shared;
    using Microsoft.EntityFrameworkCore;

    namespace Infrastructure.Repositories
    {
        public sealed class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
        {
            public VehicleRepository(AppDbContext context) : base(context)
            {
            }

            #region IVehicleUniquenessChecker Implementation

            /// <summary>
            /// Check if plate number is unique (synchronous)
            /// </summary>
            public bool IsPlateUnique(VehiclePlateNumber plateNumber)
            {
                return !_dbSet
                    .AsNoTracking()
                    .Any(v => v.PlateNumber == plateNumber);
            }

            /// <summary>
            /// Check if plate number is unique (async)
            /// </summary>
            public async Task<bool> IsPlateUniqueAsync(
                VehiclePlateNumber plateNumber,
                CancellationToken cancellationToken = default)
            {
                return !await _dbSet
                    .AsNoTracking()
                    .AnyAsync(v => v.PlateNumber == plateNumber, cancellationToken);
            }

            #endregion

            #region IVehicleRepository Implementation

            public async Task<Vehicle?> GetByIdAsync(VehicleId id, CancellationToken cancellationToken = default)
            {
                return await _dbSet
                    .Include(v => v.MaintenanceSchedules)
                    .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
            }

            public async Task<Vehicle?> GetByPlateNumberAsync(
                VehiclePlateNumber plateNumber,
                CancellationToken cancellationToken = default)
            {
                return await _dbSet
                    .Include(v => v.MaintenanceSchedules)
                    .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber, cancellationToken);
            }

            public async Task<bool> ExistsAsync(
                VehiclePlateNumber plateNumber,
                CancellationToken cancellationToken = default)
            {
                return await _dbSet
                    .AsNoTracking()
                    .AnyAsync(v => v.PlateNumber == plateNumber, cancellationToken);
            }

            public async Task<IReadOnlyList<Vehicle>> GetAvailableVehiclesAsync(
                CancellationToken cancellationToken = default)
            {
                return await _dbSet
                    .AsNoTracking()
                    .Where(v => v.Status == VehicleStatus.Available)
                    .OrderBy(v => v.PlateNumber.Value)
                    .ToListAsync(cancellationToken);
            }

            public async Task<IReadOnlyList<Vehicle>> GetByStatusAsync(
                VehicleStatus status,
                CancellationToken cancellationToken = default)
            {
                return await _dbSet
                    .AsNoTracking()
                    .Where(v => v.Status == status)
                    .OrderBy(v => v.PlateNumber.Value)
                    .ToListAsync(cancellationToken);
            }

            public async Task<IReadOnlyList<Vehicle>> GetVehiclesDueForMaintenanceAsync(
                DateTime date,
                CancellationToken cancellationToken = default)
            {
                return await _dbSet
                    .AsNoTracking()
                    .Include(v => v.MaintenanceSchedules)
                    .Where(v => v.MaintenanceSchedules
                        .Any(m => m.ScheduledDate.Date == date.Date))
                    .ToListAsync(cancellationToken);
            }

            public async Task<IReadOnlyList<Vehicle>> GetVehiclesWithUpcomingMaintenanceAsync(
                int daysAhead = 7,
                CancellationToken cancellationToken = default)
            {
                var startDate = DateTime.UtcNow.Date;
                var endDate = startDate.AddDays(daysAhead);

                return await _dbSet
                    .AsNoTracking()
                    .Include(v => v.MaintenanceSchedules)
                    .Where(v => v.MaintenanceSchedules
                        .Any(m => m.ScheduledDate >= startDate && m.ScheduledDate <= endDate))
                    .OrderBy(v => v.MaintenanceSchedules
                        .Where(m => m.ScheduledDate >= startDate && m.ScheduledDate <= endDate)
                        .Min(m => m.ScheduledDate))
                    .ToListAsync(cancellationToken);
            }

            public async Task<IReadOnlyList<Vehicle>> SearchBySpecificationAsync(
                string? model = null,
                int? year = null,
                string? engineType = null,
                CancellationToken cancellationToken = default)
            {
                var query = _dbSet.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(model))
                {
                    query = query.Where(v => v.Specification.Model.ToLower().Contains(model.ToLower()));
                }

                if (year.HasValue)
                {
                    query = query.Where(v => v.Specification.Year == year.Value);
                }

                if (!string.IsNullOrWhiteSpace(engineType))
                {
                    query = query.Where(v => v.Specification.EngineType.ToLower().Contains(engineType.ToLower()));
                }

                return await query
                    .OrderBy(v => v.Specification.Model)
                    .ThenBy(v => v.Specification.Year)
                    .ToListAsync(cancellationToken);
            }

            public async Task<(IReadOnlyList<Vehicle> Items, int TotalCount)> GetPagedAsync(
                int pageNumber,
                int pageSize,
                VehicleStatus? status = null,
                CancellationToken cancellationToken = default)
            {
                var query = _dbSet.AsNoTracking();

                if (status.HasValue)
                {
                    query = query.Where(v => v.Status == status.Value);
                }

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .OrderBy(v => v.PlateNumber.Value)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return (items, totalCount);
            }

            #endregion
        }
    }
}