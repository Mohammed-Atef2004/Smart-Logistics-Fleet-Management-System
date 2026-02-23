using Application.Features.Driver.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Queries.GetAll
{
    public record GetAllDriversQuery: IRequest<List<DriverDto>>;
}
