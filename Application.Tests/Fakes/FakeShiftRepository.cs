using Domain.Drivers.ValueObjects;
using Domain.Shifts;
using Domain.Shifts.Enums;
using Domain.Shifts.ValueObjects;

namespace Application.Tests.Fakes;

/// <summary>
/// In-memory IShiftRepository for handler tests. Entities are stored and
/// returned by reference so tests can assert on aggregate state after a
/// handler runs (same tracking semantics as EF Core).
/// <para>
/// <see cref="HasOverlappingShift"/> mirrors the SQL predicate, so tests
/// exercise the real overlap rules instead of a hard-coded flag.
/// </para>
/// </summary>
public sealed class FakeShiftRepository : IShiftRepository
{
    private readonly Dictionary<Guid, Shift> _shifts = [];

    /// <summary>Shifts passed to <see cref="AddAsync"/> by the code under test.</summary>
    public List<Shift> Added { get; } = [];

    public IQueryable<Shift> EntityQuery => _shifts.Values.AsQueryable();

    public FakeShiftRepository(params Shift[] seed)
    {
        foreach (var shift in seed)
            _shifts[shift.Id.Value] = shift;
    }

    /// <summary>Adds a pre-existing shift without recording it in <see cref="Added"/>.</summary>
    public void Seed(Shift shift) => _shifts[shift.Id.Value] = shift;

    public Task<Shift?> GetByIdAsync(ShiftId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_shifts.TryGetValue(id.Value, out var shift) ? shift : null);

    public Task<IReadOnlyList<Shift>> ListAsync(
        Guid? driverId = null,
        ShiftStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _shifts.Values.AsEnumerable();

        if (driverId is { } driver)
            query = query.Where(s => s.DriverId.Value == driver);

        if (status is { } shiftStatus)
            query = query.Where(s => s.Status == shiftStatus);

        IReadOnlyList<Shift> result = query
            .OrderBy(s => s.ShiftStart)
            .ToList();

        return Task.FromResult(result);
    }

    public Task AddAsync(Shift entity)
    {
        _shifts[entity.Id.Value] = entity;
        Added.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Shift entity) => _shifts[entity.Id.Value] = entity;

    public Task<bool> HasOverlappingShift(
        DriverId driverId,
        DateTime start,
        DateTime end,
        ShiftId? excludingShiftId = null)
        => Task.FromResult(_shifts.Values.Any(s =>
            s.DriverId == driverId &&
            s.Status != ShiftStatus.Cancelled &&
            s.Status != ShiftStatus.Aborted &&
            s.ShiftEnd > start &&
            s.ShiftStart < end &&
            (excludingShiftId is null || s.Id != excludingShiftId.Value)));
}
