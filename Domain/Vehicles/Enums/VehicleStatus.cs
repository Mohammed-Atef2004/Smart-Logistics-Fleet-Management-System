namespace Domain.Vehicles.Enums;

/// <summary>
/// Vehicle lifecycle status.
///
/// The old list mixed two vocabularies (Available/OnTrip/Active vs
/// InUse/OutOfService) and four members were never referenced anywhere
/// (OnTrip, OutOfService, Active). They were removed; the numeric values
/// of the surviving members are intentionally left UNCHANGED (Available=1,
/// InMaintenance=3, Retired=6, InUse=7) because API clients may send
/// these numbers to the status endpoint and existing rows store them.
/// Gaps in the sequence are deliberate — do not "fill" them.
///
/// Legal transitions are enforced by the Vehicle aggregate state machine.
/// </summary>
public enum VehicleStatus
{
    Available = 1,
    InMaintenance = 3,
    Retired = 6,
    InUse = 7,
}
