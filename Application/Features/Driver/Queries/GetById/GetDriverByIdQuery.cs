using Application.Features.Driver.DTOs;
using Domain.Drivers.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Queries.GetById
{
    public record GetDriverByIdQuery(DriverId Id): IRequest<DriverDto>;

}
