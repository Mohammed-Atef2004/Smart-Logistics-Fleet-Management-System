using Domain.Fleet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IDriverRepository:IGenericRepository<Driver>
    {
        Task Update(Driver driver);
    }
}
