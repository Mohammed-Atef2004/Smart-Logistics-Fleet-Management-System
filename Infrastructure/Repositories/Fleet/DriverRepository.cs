using Domain.Fleet;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Repositories.Fleet;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories;
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