using Domain.SharedKernel;
using Domain.Warehouse;
using MediatR;

namespace Application.Warehouses.GetAllWarehouses
{
    // ──────────────────────────── Handler ────────────────────────────

    public sealed class GetAllWarehousesQueryHandler
        : IRequestHandler<GetAllWarehousesQuery, Result<IReadOnlyList<GetAllWarehousesResponse>>>
    {
        private readonly IWarehouseRepository _readRepository;

        public GetAllWarehousesQueryHandler(IWarehouseRepository readRepository)
            => _readRepository = readRepository;

        public async Task<Result<IReadOnlyList<GetAllWarehousesResponse>>> Handle(
            GetAllWarehousesQuery query,
            CancellationToken cancellationToken)
        {
            var warehouses = _readRepository.EntityQuery.Where(q=>q.IsActive==query.ActiveOnly);
            var response = warehouses.Select(w => new GetAllWarehousesResponse(
                w.Id,
                w.Name,
                w.IsActive,
                w.Address.City,
                w.Address.Country,
                w.StorageLocations.Count
            )).ToList();

            return Result<IReadOnlyList<GetAllWarehousesResponse>>.Success(response);
        }
    }
}