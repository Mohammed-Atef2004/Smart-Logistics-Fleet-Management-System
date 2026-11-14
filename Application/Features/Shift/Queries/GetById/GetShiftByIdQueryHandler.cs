using Domain.Shifts;
using Application.Features.Shift.DTOs;
using Microsoft.EntityFrameworkCore;

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