using Application.Common.Interfaces;
using Application.Features.Shift.DTOs;
using AutoMapper;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shift.Queries.GetById
{
    public sealed class GetShiftByIdQueryHandler : IRequestHandler<GetShiftByIdQuery, Result<ShiftDto>>
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IMapper _mapper;
        public GetShiftByIdQueryHandler(IShiftRepository shiftRepository, IMapper mapper)
        {
            _shiftRepository = shiftRepository;
            _mapper = mapper;
        }
        public async Task<Result<ShiftDto>> Handle(GetShiftByIdQuery request, CancellationToken cancellationToken)
        {
            var shift = await _shiftRepository.EntityQuery.SingleOrDefaultAsync(x => x.Id == request.shiftId);
            if (shift is null) throw new Exception("Shift not found");
            return _mapper.Map<Result<ShiftDto>>(shift);
        }
    }
}