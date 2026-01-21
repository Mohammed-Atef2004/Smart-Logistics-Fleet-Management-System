using Domain.Fleet.Entities;
using Domain.Fleet.Interfaces;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DriverRepository : GenericRepository<Driver>, IDriverRepository
    {
        public DriverRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void Update(Driver driver)
        {
            _context.Drivers.Update(driver);
        }

       
    }
}