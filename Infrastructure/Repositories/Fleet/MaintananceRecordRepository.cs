using Domain.Fleet;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Repositories.Fleet;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Fleet
{
    public class MaintenanceRecordRepository : GenericRepository<MaintenanceRecord>, IMaintenanceRecordRepository
    {
        public MaintenanceRecordRepository(ApplicationDbContext context) : base(context)
        {
        }
        public void Update(MaintenanceRecord maintenanceRecord)
        {
            _context.MaintenanceRecords.Update(maintenanceRecord);
        }
    }
    
}
