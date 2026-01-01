using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories.Users
{
    public interface IRoleRepository:IGenericRepository<Role>
    {
        Task Update(Role role);
    }
}
