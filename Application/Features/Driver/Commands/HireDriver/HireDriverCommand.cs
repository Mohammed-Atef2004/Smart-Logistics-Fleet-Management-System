using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.HireDriver
{
    public record HireDriverCommand(
        string FullName,
        string LicenseNumber,
        DateTime ExpiryDate,
        string Category
    ) : IRequest<Guid>;
}
