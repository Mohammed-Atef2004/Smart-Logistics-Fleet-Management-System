using Application.Features.Shipment.DTOs;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Queries.GetAll
{
    public record GetAllShipmentsQuery: IRequest<Result<List<ShipmentListDto>>>;
}
