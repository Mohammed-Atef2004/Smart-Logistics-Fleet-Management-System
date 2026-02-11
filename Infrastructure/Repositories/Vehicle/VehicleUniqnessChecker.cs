using Domain.Vehicles;
using Infrastructure.Presistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Vehicle
{
    using Domain.Interfaces.Repositories;
    using Domain.Vehicles.ValueObjects;
    using Microsoft.EntityFrameworkCore;

    namespace Infrastructure.Repositories
    {
        /// <summary>
        /// Implementation of vehicle uniqueness checking
        /// </summary>
        public sealed class VehicleUniquenessChecker : IVehicleUniquenessChecker
        {
            private readonly AppDbContext _context;

            public VehicleUniquenessChecker(AppDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public bool IsPlateUnique(VehiclePlateNumber plateNumber)
            {
                // Synchronous version for domain logic
                return !_context.Vehicles
                    .AsNoTracking()
                    .Any(v => v.PlateNumber == plateNumber);
            }

            public async Task<bool> IsPlateUniqueAsync(
                VehiclePlateNumber plateNumber,
                CancellationToken cancellationToken = default)
            {
                return !await _context.Vehicles
                    .AsNoTracking()
                    .AnyAsync(v => v.PlateNumber == plateNumber, cancellationToken);
            }
        }
    }
}
