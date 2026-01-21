using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Users.Interfaces.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email, IEnumerable<string> roles);
    }

}
