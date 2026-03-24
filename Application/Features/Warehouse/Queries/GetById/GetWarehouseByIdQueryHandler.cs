using Domain.SharedKernel;
using Domain.Warehouse;
using Domain.Warehouse.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Application.Warehouses.GetWarehouseById
{

    public sealed class GetWarehouseByIdQueryHandler
        : IRequestHandler<GetWarehouseByIdQuery, Result<GetWarehouseByIdResponse>>
    {
        private readonly IWarehouseRepository _readRepository;

        public GetWarehouseByIdQueryHandler(IWarehouseRepository readRepository)
            => _readRepository = readRepository;

        public async Task<Result<GetWarehouseByIdResponse>> Handle(
            GetWarehouseByIdQuery query,
            CancellationToken cancellationToken)
        {
            var warehouse = await _readRepository.EntityQuery.FirstOrDefaultAsync(q=>q.Id==query.Id, cancellationToken);

            if (warehouse is null)
                return Result<GetWarehouseByIdResponse>.Failure(WarehouseErrors.NotFound);

            var response = new GetWarehouseByIdResponse(
                warehouse.Id,
                warehouse.Name,
                warehouse.IsActive,
                warehouse.Address.Street,
                warehouse.Address.City,
                warehouse.Address.Country,
                warehouse.Address.ZipCode,
                warehouse.StorageLocations.Select(sl => new StorageLocationResponse(
                    sl.Id,
                    sl.Name,
                    sl.IsActive,
                    sl.Capacity.MaxSlots,
                    sl.Capacity.UsedSlots,
                    sl.Capacity.AvailableSlots,
                    sl.AssignedItems.Select(x => x).ToList()
                )).ToList() );

            return Result<GetWarehouseByIdResponse>.Success(response);
        }
    }
}