using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Roles
{
    public interface IRoleRepository:IGenericRepository<Role>
    {
        Task Update(Role role);
    }
}
