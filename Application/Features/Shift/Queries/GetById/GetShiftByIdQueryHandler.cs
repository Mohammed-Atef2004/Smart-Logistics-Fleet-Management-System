using Application.Features.Shift.DTOs;
using Domain.Shifts;
using Domain.Shifts.Errors;
using Domain.Shifts.ValueObjects;

namespace Application.Features.Shift.Queries.GetById;

public sealed class GetShiftByIdQueryHandler : IRequestHandler<GetShiftByIdQuery, Result<ShiftDto>>
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IMapper _mapper;

    public GetShiftByIdQueryHandler(IShiftRepository shiftRepository, IMapper mapper)
    {
        _shiftRepository = shiftRepository;
        _mapper = mapper;
    }

    public async Task<Result<ShiftDto>> Handle(
        GetShiftByIdQuery request,
        CancellationToken cancellationToken)
    {
        var id = new ShiftId(request.ShiftId);

        // Regression: the old handler threw a raw Exception for a missing
        // shift, which surfaced as a 500 instead of a 404-style failure.
        var shift = await _shiftRepository.GetByIdAsync(id, cancellationToken);
        if (shift is null)
            return Result<ShiftDto>.Failure(ShiftErrors.NotFound(request.ShiftId));

        return Result<ShiftDto>.Success(_mapper.Map<ShiftDto>(shift));
    }
}
