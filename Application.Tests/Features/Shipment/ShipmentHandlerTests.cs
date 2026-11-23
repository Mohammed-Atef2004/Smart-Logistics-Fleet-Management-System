using Application.Features.Shipment.Commands.AddPackage;
using Application.Features.Shipment.Commands.AddRoutePoint;
using Application.Features.Shipment.Commands.AssignCarrier;
using Application.Features.Shipment.Commands.Cancel;
using Application.Features.Shipment.Commands.Create;
using Application.Features.Shipment.Commands.Dispatch;
using Application.Features.Shipment.Commands.MarkDelivered;
using Application.Features.Shipment.Commands.MarkDeliveryFailed;
using Application.Features.Shipment.Commands.RemovePackage;
using Application.Features.Shipment.Commands.UpdateDeliveryAddress;
using Application.Features.Shipment.Mappings;
using Application.Features.Shipment.Queries.GetAll;
using Application.Features.Shipment.Queries.GetById;
using Application.Features.Shipment.Queries.GetPackages;
using Application.Tests.Fakes;
using AutoMapper;
using Domain.Shipments.Enums;
using Domain.Shipments.Errors;
using Domain.Shipments.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using ShipmentAggregate = Domain.Shipments.Shipment;

namespace Application.Tests.Features.Shipment;

/// <summary>
/// Handler-level use-case tests for the Shipment module (fakes instead of EF
/// Core). Covers creation, package/route management, dispatch, delivery,
/// cancellation, address updates, and the read side including mapping.
/// </summary>
public class ShipmentHandlerTests
{
    private readonly FakeShipmentRepository _shipments = new();
    private readonly FakeUnitOfWork _uow = new();

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    private static DeliveryAddress Address() =>
        DeliveryAddress.Create("123 Main St", "Cairo", "Cairo Gov", "11511", "Egypt").Value;

    private static ShipmentAggregate NewShipment(string trackingNumber = "TRK-001") =>
        ShipmentAggregate.Create("SND-123", Address(), trackingNumber).Value;

    private ShipmentAggregate Seed(string trackingNumber = "TRK-001")
    {
        var shipment = NewShipment(trackingNumber);
        _shipments.Seed(shipment);
        return shipment;
    }

    private static CreateShipmentCommand CreateCommand(
        string trackingNumber = "TRK-001",
        string senderId = "SND-123") =>
        new(senderId, "123 Main St", "Cairo", "Cairo Gov", "11511", "Egypt", null, trackingNumber);

    private static AddPackageCommand AddPackageCommand(Guid shipmentId, decimal weight = 5m) =>
        new(
            shipmentId,
            "Laptop",
            weight,
            WeightUnit.Kg,
            10m,
            10m,
            10m,
            DimensionUnit.Cm,
            "Electronics",
            false,
            false,
            1000m,
            "USD");

    private static IMapper Mapper() =>
        new MapperConfiguration(
                cfg => cfg.AddProfile<ShipmentProfile>(),
                NullLoggerFactory.Instance)
            .CreateMapper();

    private static ShipmentAggregate DispatchedShipment()
    {
        var shipment = NewShipment();
        shipment.AddPackage("Laptop", Weight.InKilograms(5m), Dimensions.InCentimeters(10, 10, 10));
        shipment.AssignCarrier("Aramex");
        shipment.Dispatch();
        return shipment;
    }

    // ---------------------------------------------------------------
    // Create
    // ---------------------------------------------------------------

    [Fact]
    public async Task Create_Should_Add_Shipment_And_Return_Id()
    {
        var result = await new CreateShipmentCommandHandler(_uow, _shipments)
            .Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        _shipments.Added.Should().ContainSingle();
        _shipments.Added[0].Id.Value.Should().Be(result.Value);
        _uow.SaveCount.Should().Be(1);
    }

    [Fact]
    public async Task Create_Should_Fail_When_Tracking_Number_Already_Exists()
    {
        Seed("TRK-001");

        var result = await new CreateShipmentCommandHandler(_uow, _shipments)
            .Handle(CreateCommand("TRK-001"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Shipment.DuplicateTrackingNumber");
        _shipments.Added.Should().BeEmpty();
        _uow.SaveCount.Should().Be(0);
    }

    [Fact]
    public async Task Create_Should_Fail_When_Address_Is_Invalid()
    {
        var command = CreateCommand() with { City = "  " };

        var result = await new CreateShipmentCommandHandler(_uow, _shipments)
            .Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DeliveryAddress.Required");
        _shipments.Added.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_Should_Fail_When_Sender_Is_Empty()
    {
        var result = await new CreateShipmentCommandHandler(_uow, _shipments)
            .Handle(CreateCommand(senderId: "  "), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.EmptySenderId);
    }

    // ---------------------------------------------------------------
    // Packages
    // ---------------------------------------------------------------

    [Fact]
    public async Task AddPackage_Should_Return_Id_Of_The_Stored_Package()
    {
        var shipment = Seed();

        var result = await new AddPackageCommandHandler(_uow, _shipments)
            .Handle(AddPackageCommand(shipment.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.Packages.Should().ContainSingle();
        shipment.Packages[0].Id.Value.Should().Be(result.Value);
        _uow.SaveCount.Should().Be(1);
    }

    [Fact]
    public async Task AddPackage_Should_Fail_When_Weight_Is_Invalid()
    {
        var shipment = Seed();

        var result = await new AddPackageCommandHandler(_uow, _shipments)
            .Handle(AddPackageCommand(shipment.Id.Value, weight: 0m), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Weight.InvalidValue);
        shipment.Packages.Should().BeEmpty();
    }

    [Fact]
    public async Task AddPackage_Should_Fail_When_Shipment_Is_Not_Found()
    {
        var result = await new AddPackageCommandHandler(_uow, _shipments)
            .Handle(AddPackageCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Shipment.NotFound");
    }

    [Fact]
    public async Task AddPackage_Should_Fail_When_Shipment_Is_No_Longer_Editable()
    {
        var shipment = DispatchedShipment();
        _shipments.Seed(shipment);

        var result = await new AddPackageCommandHandler(_uow, _shipments)
            .Handle(AddPackageCommand(shipment.Id.Value), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.NotEditable);
        shipment.Packages.Should().ContainSingle();
    }

    [Fact]
    public async Task RemovePackage_Should_Remove_From_Shipment()
    {
        var shipment = Seed();
        var packageId = shipment
            .AddPackage("Laptop", Weight.InKilograms(5m), Dimensions.InCentimeters(10, 10, 10))
            .Value;

        var result = await new RemovePackageCommandHandler(_uow, _shipments)
            .Handle(new RemovePackageCommand(shipment.Id.Value, packageId.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.Packages.Should().BeEmpty();
    }

    [Fact]
    public async Task RemovePackage_Should_Fail_When_Package_Is_Missing()
    {
        var shipment = Seed();

        var result = await new RemovePackageCommandHandler(_uow, _shipments)
            .Handle(new RemovePackageCommand(shipment.Id.Value, Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Package.NotFound");
    }

    // ---------------------------------------------------------------
    // Carrier & dispatch
    // ---------------------------------------------------------------

    [Fact]
    public async Task AssignCarrier_Should_Set_Carrier_And_Estimated_Date()
    {
        var shipment = Seed();
        var eta = DateTime.UtcNow.AddDays(3);

        var result = await new AssignCarrierCommandHandler(_uow, _shipments)
            .Handle(new AssignCarrierCommand(shipment.Id.Value, "FedEx", eta), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.Tracking.CarrierName.Should().Be("FedEx");
        shipment.Tracking.EstimatedDeliveryDate.Should().Be(eta);
    }

    [Fact]
    public async Task AssignCarrier_Should_Fail_When_Name_Is_Empty()
    {
        var shipment = Seed();

        var result = await new AssignCarrierCommandHandler(_uow, _shipments)
            .Handle(new AssignCarrierCommand(shipment.Id.Value, "  "), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.EmptyCarrier);
    }

    [Fact]
    public async Task Dispatch_Should_Fail_Without_Packages()
    {
        var shipment = Seed();
        shipment.AssignCarrier("Aramex");

        var result = await new DispatchCommandHandler(_uow, _shipments)
            .Handle(new DispatchCommand(shipment.Id.Value), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.NoPackages);
    }

    [Fact]
    public async Task Dispatch_Should_Move_Shipment_To_InTransit()
    {
        var shipment = Seed();
        shipment.AddPackage("Laptop", Weight.InKilograms(5m), Dimensions.InCentimeters(10, 10, 10));
        shipment.AssignCarrier("Aramex");

        var result = await new DispatchCommandHandler(_uow, _shipments)
            .Handle(new DispatchCommand(shipment.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.Status.Should().Be(ShipmentStatus.InTransit);
    }

    // ---------------------------------------------------------------
    // Route points
    // ---------------------------------------------------------------

    [Fact]
    public async Task AddRoutePoint_Should_Update_Status()
    {
        var shipment = Seed();

        var result = await new AddRoutePointCommandHandler(_uow, _shipments)
            .Handle(
                new AddRoutePointCommand(
                    shipment.Id.Value, "Cairo Hub", "Processing", DateTime.UtcNow, RoutePointType.OutForDelivery),
                CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.Route.Should().ContainSingle();
        shipment.Status.Should().Be(ShipmentStatus.OutForDelivery);
    }

    [Fact]
    public async Task AddRoutePoint_Should_Fail_When_Location_Is_Empty()
    {
        var shipment = Seed();

        var result = await new AddRoutePointCommandHandler(_uow, _shipments)
            .Handle(
                new AddRoutePointCommand(
                    shipment.Id.Value, "  ", "Processing", DateTime.UtcNow, RoutePointType.Transit),
                CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("RoutePoint.EmptyLocation");
        shipment.Route.Should().BeEmpty();
    }

    // ---------------------------------------------------------------
    // Delivery
    // ---------------------------------------------------------------

    [Fact]
    public async Task MarkDelivered_Should_Fail_From_Created()
    {
        var shipment = Seed();

        var result = await new MarkShipmentDeliveredCommandHandler(_uow, _shipments)
            .Handle(
                new MarkShipmentDeliveredCommand(shipment.Id.Value, DateTime.UtcNow),
                CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.InvalidStatusTransition);
    }

    [Fact]
    public async Task MarkDelivered_Should_Deliver_A_Shipment_In_Transit()
    {
        var shipment = DispatchedShipment();
        _shipments.Seed(shipment);

        var result = await new MarkShipmentDeliveredCommandHandler(_uow, _shipments)
            .Handle(
                new MarkShipmentDeliveredCommand(shipment.Id.Value, DateTime.UtcNow, "Reception"),
                CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.Status.Should().Be(ShipmentStatus.Delivered);
        shipment.DeliveredAt.Should().NotBeNull();
    }

    [Fact]
    public async Task MarkDeliveryFailed_Should_Fail_From_Created()
    {
        var shipment = Seed();

        var result = await new MarkShipmentDeliveryFailedCommandHandler(_uow, _shipments)
            .Handle(
                new MarkShipmentDeliveryFailedCommand(shipment.Id.Value, "Nobody home"),
                CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.InvalidStatusTransition);
    }

    [Fact]
    public async Task MarkDeliveryFailed_Should_Mark_A_Shipment_In_Transit()
    {
        var shipment = DispatchedShipment();
        _shipments.Seed(shipment);

        var result = await new MarkShipmentDeliveryFailedCommandHandler(_uow, _shipments)
            .Handle(
                new MarkShipmentDeliveryFailedCommand(shipment.Id.Value, "Nobody home"),
                CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.Status.Should().Be(ShipmentStatus.Failed);
    }

    // ---------------------------------------------------------------
    // Cancellation & address
    // ---------------------------------------------------------------

    [Fact]
    public async Task Cancel_Should_Cancel_A_Shipment()
    {
        var shipment = Seed();

        var result = await new CancelShipmentCommandHandler(_uow, _shipments)
            .Handle(
                new CancelShipmentCommand(shipment.Id.Value, "Customer request", "Admin"),
                CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.Status.Should().Be(ShipmentStatus.Cancelled);
    }

    [Fact]
    public async Task Cancel_Should_Fail_When_Reason_Is_Empty()
    {
        var shipment = Seed();

        var result = await new CancelShipmentCommandHandler(_uow, _shipments)
            .Handle(
                new CancelShipmentCommand(shipment.Id.Value, "  ", "Admin"),
                CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.EmptyCancelReason);
    }

    [Fact]
    public async Task UpdateAddress_Should_Replace_Address()
    {
        var shipment = Seed();

        var result = await new UpdateShipmentDeliveryAddressCommandHandler(_uow, _shipments)
            .Handle(
                new UpdateShipmentDeliveryAddressCommand(
                    shipment.Id.Value, "New St", "Giza", "Giza Gov", "12345", "Egypt", "Apt 4"),
                CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.DestinationAddress.City.Should().Be("Giza");
        shipment.DestinationAddress.ApartmentUnit.Should().Be("Apt 4");
    }

    [Fact]
    public async Task UpdateAddress_Should_Fail_When_Address_Is_Invalid()
    {
        var shipment = Seed();

        var result = await new UpdateShipmentDeliveryAddressCommandHandler(_uow, _shipments)
            .Handle(
                new UpdateShipmentDeliveryAddressCommand(
                    shipment.Id.Value, "New St", "  ", "Giza Gov", "12345", "Egypt"),
                CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DeliveryAddress.Required");
    }

    // ---------------------------------------------------------------
    // Queries + mapping
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetById_Should_Return_Flat_Details()
    {
        var shipment = Seed();
        var packageId = shipment
            .AddPackage("Laptop", Weight.InKilograms(5m), Dimensions.InCentimeters(10, 10, 10), declaredValue: 750m)
            .Value;

        var result = await new GetShipmentByIdQueryHandler(_shipments, Mapper())
            .Handle(new GetShipmentByIdQuery(shipment.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value;
        dto.Id.Should().Be(shipment.Id.Value);
        dto.TrackingNumber.Should().Be("TRK-001");
        dto.Status.Should().Be("Created");
        dto.SenderId.Should().Be("SND-123");
        dto.DestinationCity.Should().Be("Cairo");
        dto.TotalPackages.Should().Be(1);
        dto.TotalWeightKg.Should().Be(5m);
        dto.TotalDeclaredValue.Should().Be(750m);
        dto.Packages.Should().ContainSingle();
        dto.Packages[0].Id.Should().Be(packageId.Value);
        dto.Packages[0].WeightKg.Should().Be(5m);
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound()
    {
        var result = await new GetShipmentByIdQueryHandler(_shipments, Mapper())
            .Handle(new GetShipmentByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Shipment.NotFound");
    }

    [Fact]
    public async Task GetAll_Should_Filter_By_Status()
    {
        Seed("TRK-001");
        var cancelled = NewShipment("TRK-002");
        cancelled.Cancel("Customer request", "Admin");
        _shipments.Seed(cancelled);

        var handler = new GetAllShipmentsQueryHandler(_shipments, Mapper());

        var all = await handler.Handle(new GetAllShipmentsQuery(), CancellationToken.None);
        var onlyCancelled = await handler.Handle(
            new GetAllShipmentsQuery(ShipmentStatus.Cancelled), CancellationToken.None);

        all.IsSuccess.Should().BeTrue();
        all.Value.Should().HaveCount(2);
        onlyCancelled.Value.Should().ContainSingle()
            .Which.TrackingNumber.Should().Be("TRK-002");
    }

    [Fact]
    public async Task GetPackages_Should_Return_Empty_List_When_Shipment_Has_No_Packages()
    {
        var shipment = Seed();

        var result = await new GetShipmentPackagesQueryHandler(_shipments, Mapper())
            .Handle(new GetShipmentPackagesQuery(shipment.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPackages_Should_Return_NotFound_When_Shipment_Is_Missing()
    {
        var result = await new GetShipmentPackagesQueryHandler(_shipments, Mapper())
            .Handle(new GetShipmentPackagesQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Shipment.NotFound");
    }

    [Fact]
    public void ShipmentProfile_Configuration_Should_Be_Valid()
    {
        // Regression: the old profile used convention mapping plus ReverseMap,
        // which cannot map ShipmentId/PackageId or the nested value objects.
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<ShipmentProfile>(),
            NullLoggerFactory.Instance);

        configuration.AssertConfigurationIsValid();
    }
}
