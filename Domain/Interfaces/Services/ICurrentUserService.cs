using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    // This service provides information about the currently authenticated user.
    public interface ICurrentUserService
    {
        string? UserId { get; }
    }
}
