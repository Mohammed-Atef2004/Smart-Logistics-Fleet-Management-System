using Application.Common.Interfaces;
using Application.Features.Shift.DTOs;
using AutoMapper;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shift.Queries.GetAll
{
    public sealed class GetAllShiftsQueryHandler : IRequestHandler<GetAllShiftsQuery,Result< List<ShiftDto>>>
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IMapper _mapper;
        public GetAllShiftsQueryHandler(IShiftRepository shiftRepository, IMapper mapper)
        {
            _shiftRepository = shiftRepository;
            _mapper = mapper;
        }
        public async Task<Result<List<ShiftDto>>> Handle(GetAllShiftsQuery request, CancellationToken cancellationToken)
        {
            var shifts = _shiftRepository.EntityQuery.Select(x => x);
            return _mapper.Map<Result<List<ShiftDto>>>(shifts);
        }
    }
}