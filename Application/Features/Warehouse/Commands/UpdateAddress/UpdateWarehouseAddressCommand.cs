
using Domain.SharedKernel;
using MediatR;

namespace Application.Warehouses.UpdateAddress
{

    public sealed record UpdateWarehouseAddressCommand(
        Guid WarehouseId,
        string Street,
        string City,
        string Country,
        string ZipCode) : IRequest<Result>;
}