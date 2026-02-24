using Application.Features.Shift.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shift.Queries.GetAll
{
    public class GetAllShiftsQueryHandler : IRequestHandler<GetAllShiftsQuery,Result< List<ShiftDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllShiftsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<List<ShiftDto>>> Handle(GetAllShiftsQuery request, CancellationToken cancellationToken)
        {
            var shifts = _unitOfWork.Shifts.EntityQuery.Select(x => x);
            return _mapper.Map<Result<List<ShiftDto>>>(shifts);
        }
    }
}
