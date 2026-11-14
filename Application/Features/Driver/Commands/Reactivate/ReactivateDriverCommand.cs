using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.Reactivate
{
    public record ReactivateDriverCommand(DriverId Id) : IRequest<Unit>;

}