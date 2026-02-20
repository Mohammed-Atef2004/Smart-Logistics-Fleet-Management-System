using Domain.Interfaces.Repositories;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Drivers
{
    public interface IDriverRepository:IGenericRepository<Driver>
    {
    }
}
