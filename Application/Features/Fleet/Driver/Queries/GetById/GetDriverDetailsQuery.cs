using Application.Features.Fleet.Driver.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Driver.Queries.GetById
{
    public record GetDriverDetailsQuery(Guid Id): IRequest<DriverDto>;

}
