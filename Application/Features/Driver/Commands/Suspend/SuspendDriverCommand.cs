using Domain.Drivers.Enums;
using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.Susbend
{
    public record SuspendDriverCommand(DriverId Id, DriverSuspensionReason Reason) : IRequest<Unit>;

}