using Application.Features.Fleet.Vehicle.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Queries.GetAll
{
    public class GetAllVehiclesQueryHandler: IRequestHandler<GetAllVehiclesQuery, List<VehicleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllVehiclesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<VehicleDto>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
        {
            var vehicles=_unitOfWork.Vehicles.GetAllVehiclesWithMaintenanceAsync();
            if (vehicles == null || vehicles.Result.Count == 0)
            {
                throw new Exception("No vehicles found");
            }
            return _mapper.Map<List<VehicleDto>>(vehicles.Result);


        }
    }
}
