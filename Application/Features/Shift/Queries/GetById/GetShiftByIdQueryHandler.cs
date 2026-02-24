using Application.Features.Shift.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
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
    public class GetShiftByIdQueryHandler : IRequestHandler<GetShiftByIdQuery, Result<ShiftDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetShiftByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<ShiftDto>> Handle(GetShiftByIdQuery request, CancellationToken cancellationToken)
        {
            var shift = await _unitOfWork.Shifts.EntityQuery.SingleOrDefaultAsync(x => x.Id == request.shiftId);
            if (shift is null) throw new Exception("Shift not found");
            return _mapper.Map<Result<ShiftDto>>(shift);
        }
    }
}
