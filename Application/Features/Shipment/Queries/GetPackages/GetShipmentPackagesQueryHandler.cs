using Application.Features.Shipment.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Queries.GetPackages
{
    public class GetShipmentPackagesQueryHandler :IRequestHandler<GetShipmentPackagesQuery, Result<List<PackageDto>>>
    {
        private readonly IUnitOfWork _uintOfWork;
        private readonly IMapper _mapper;
        public GetShipmentPackagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uintOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<Result<List<PackageDto>>> Handle(GetShipmentPackagesQuery request, CancellationToken cancellationToken)
        {
           var Shipment = _uintOfWork.Shipments.EntityQuery.Where(x => x.Id == request.Id).Include(x=>x.Packages);
           var packages = Shipment.FirstOrDefault()?.Packages;
            if (packages is null || !packages.Any())
                return Task.FromResult(Result<List<PackageDto>>.Failure(new Error("Shipment.NotFound", "Shipment not found.")));
            var packageDtos = _mapper.Map<List<PackageDto>>(packages);
            return Task.FromResult(Result<List<PackageDto>>.Success(packageDtos));
        }
    }

}
