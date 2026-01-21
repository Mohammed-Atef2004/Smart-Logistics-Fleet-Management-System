using Domain.Interfaces.Repositories;
using Domain.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Users.Interfaces.Repositories
{
    public interface IUserRepository:IGenericRepository<ApplicationUser>
    {
        Task Update(ApplicationUser applicationUser);
    }
}
