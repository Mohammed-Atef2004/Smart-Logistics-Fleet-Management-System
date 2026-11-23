using Application.Features.Shift.DTOs;
using Domain.Shifts;

namespace Application.Features.Shift.Queries.GetAll;

public sealed class GetAllShiftsQueryHandler : IRequestHandler<GetAllShiftsQuery, Result<List<ShiftDto>>>
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IMapper _mapper;

    public GetAllShiftsQueryHandler(IShiftRepository shiftRepository, IMapper mapper)
    {
        _shiftRepository = shiftRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<ShiftDto>>> Handle(
        GetAllShiftsQuery request,
        CancellationToken cancellationToken)
    {
        // Regression: the old handler passed a raw IQueryable to AutoMapper
        // as if it were a Result, which could never work. The repository
        // materializes asynchronously and applies the optional filters.
        var shifts = await _shiftRepository.ListAsync(
            request.DriverId, request.Status, cancellationToken);

        return Result<List<ShiftDto>>.Success(_mapper.Map<List<ShiftDto>>(shifts.ToList()));
    }
}
