namespace Domain.Drivers.Enums;

/// <summary>
/// Driver lifecycle states. Only the states the domain can actually
/// reach are kept: Active (hired and working), Suspended (temporarily
/// blocked) and Terminated (terminal — a driver who left the company).
/// The numeric gaps are intentional: the values were kept from the old
/// enum so any serialized payload keeps its meaning.
/// </summary>
public enum DriverStatus
{
    Active = 1,
    Suspended = 3,
    Terminated = 4,
}
