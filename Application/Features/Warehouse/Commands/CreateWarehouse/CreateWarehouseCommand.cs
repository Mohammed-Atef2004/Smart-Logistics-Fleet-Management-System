namespace Application.Warehouses.CreateWarehouse
{
    // ──────────────────────────── Command ────────────────────────────

    public sealed record CreateWarehouseCommand(
        string Name,
        string Street,
        string City,
        string Country,
        string ZipCode) : IRequest<Result<CreateWarehouseResponse>>;
}