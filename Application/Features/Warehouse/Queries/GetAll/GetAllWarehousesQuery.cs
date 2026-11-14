namespace Application.Warehouses.GetAllWarehouses
{
    public sealed record GetAllWarehousesQuery(bool ActiveOnly = true)
        : IRequest<Result<IReadOnlyList<GetAllWarehousesResponse>>>;
}