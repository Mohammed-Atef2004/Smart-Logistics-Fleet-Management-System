using Domain.Drivers.ValueObjects;
using Domain.Shifts;
using Domain.Shifts.Enums;
using Domain.Shifts.ValueObjects;
using Infrastructure.Presistence.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ShiftRepository : GenericRepository<Shift>,IShiftRepository
    {
        private readonly AppDbContext _db;
        public ShiftRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }   


        public Task<bool> HasActiveShift(DriverId driverId) =>
            _db.Shifts.AnyAsync(x => x.DriverId == driverId && x.Status == Domain.Shifts.ShiftStatus.Active);
        public Task<bool> HasOverlappingShift(DriverId driverId, DateTime start, DateTime end)
        {
            return _db.Shifts.AnyAsync(s =>
                s.DriverId == driverId &&
                s.Status != Domain.Shifts.ShiftStatus.Cancelled &&  // ignore cancelled shifts
                s.ShiftEnd > start &&                      // overlap check
                s.ShiftStart < end
            );
        }
    }
}
