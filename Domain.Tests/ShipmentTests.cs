using System.Reflection;
using Domain.SharedKernel;
using Domain.Shipments;
using Domain.Shipments.Enums;
using Domain.Shipments.Errors;
using Domain.Shipments.Events;
using Domain.Shipments.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.Tests;

/// <summary>
/// Domain coverage for the Shipment aggregate and its value objects: creation,
/// package/route management, the delivery lifecycle, cancellation, and the
/// invariant that the factories return <see cref="Result"/> instead of
/// throwing.
/// </summary>
public class ShipmentTests
{
    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    private static DeliveryAddress ValidAddress() =>
        DeliveryAddress.Create(
            "123 Main St",
            "Cairo",
            "Cairo Gov",
            "11511",
            "Egypt").Value;

    private static Weight ValidWeight(decimal kg = 5m) => Weight.InKilograms(kg);

    private static Dimensions ValidDimensions() => Dimensions.InCentimeters(10m, 10m, 10m);

    private static Shipment NewShipment(string trackingNumber = "TRK-001") =>
        Shipment.Create("SND-123", ValidAddress(), trackingNumber).Value;

    private static Shipment PreparedShipment()
    {
        var shipment = NewShipment();
        shipment.AddPackage("Laptop", ValidWeight(), ValidDimensions()).IsSuccess.Should().BeTrue();
        shipment.AssignCarrier("Aramex").IsSuccess.Should().BeTrue();
        return shipment;
    }

    private static Shipment DispatchedShipment()
    {
        var shipment = PreparedShipment();
        shipment.Dispatch().IsSuccess.Should().BeTrue();
        return shipment;
    }

    // ---------------------------------------------------------------
    // Create
    // ---------------------------------------------------------------

    [Fact]
    public void Create_WithValidData_Should_Create_Shipment_And_Raise_Event()
    {
        var address = ValidAddress();

        var result = Shipment.Create("SND-123", address, "TRK-001");

        result.IsSuccess.Should().BeTrue();
        var shipment = result.Value;
        shipment.Id.IsEmpty.Should().BeFalse();
        shipment.SenderId.Should().Be("SND-123");
        shipment.DestinationAddress.Should().Be(address);
        shipment.Tracking.Status.Should().Be(ShipmentStatus.Created);
        shipment.Tracking.TrackingNumber.Should().Be("TRK-001");
        shipment.Version.Should().Be(0);
        shipment.Packages.Should().BeEmpty();
        shipment.Route.Should().BeEmpty();

        shipment.DomainEvents.OfType<ShipmentCreatedEvent>()
            .Should().ContainSingle(e =>
                e.ShipmentId == shipment.Id &&
                e.SenderId == "SND-123" &&
                e.TrackingNumber == "TRK-001");
    }

    [Fact]
    public void Create_Should_Trim_Optional_Text()
    {
        var shipment = Shipment.Create(
            "  SND-123  ",
            ValidAddress(),
            "  TRK-001  ",
            recipientName: "  Mohamed  ",
            recipientPhone: "  01000000000  ",
            specialInstructions: "  Handle with care  ").Value;

        shipment.SenderId.Should().Be("SND-123");
        shipment.Tracking.TrackingNumber.Should().Be("TRK-001");
        shipment.RecipientName.Should().Be("Mohamed");
        shipment.RecipientPhone.Should().Be("01000000000");
        shipment.SpecialInstructions.Should().Be("Handle with care");
    }

    [Fact]
    public void Create_WithEmptySenderId_Should_Fail()
    {
        var result = Shipment.Create("  ", ValidAddress(), "TRK-001");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.EmptySenderId);
    }

    [Fact]
    public void Create_WithNullAddress_Should_Fail()
    {
        var result = Shipment.Create("SND-123", null!, "TRK-001");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.InvalidAddress);
    }

    [Fact]
    public void Create_WithEmptyTrackingNumber_Should_Fail()
    {
        var result = Shipment.Create("SND-123", ValidAddress(), "  ");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("TrackingInfo.EmptyTrackingNumber");
    }

    // ---------------------------------------------------------------
    // Packages
    // ---------------------------------------------------------------

    [Fact]
    public void AddPackage_WhenEditable_Should_Add_Package_And_Update_Aggregate()
    {
        var shipment = NewShipment();

        var result = shipment.AddPackage(
            "Laptop", ValidWeight(5m), ValidDimensions(), declaredValue: 1000m);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsEmpty.Should().BeFalse();
        shipment.Packages.Should().ContainSingle()
            .Which.Id.Should().Be(result.Value);
        shipment.TotalPackages.Should().Be(1);
        shipment.TotalWeightKg.Should().Be(5m);
        shipment.TotalDeclaredValue.Should().Be(1000m);
        shipment.Version.Should().Be(1);

        shipment.DomainEvents.OfType<PackageAddedToShipmentEvent>()
            .Should().ContainSingle(e => e.PackageId == result.Value);
    }

    [Fact]
    public void AddPackage_Should_Reject_Null_Weight()
    {
        var shipment = NewShipment();

        var result = shipment.AddPackage("Laptop", null!, ValidDimensions());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PackageErrors.InvalidWeight);
    }

    [Fact]
    public void AddPackage_Should_Reject_Null_Dimensions()
    {
        var shipment = NewShipment();

        var result = shipment.AddPackage("Laptop", ValidWeight(), null!);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PackageErrors.InvalidDimensions);
    }

    [Fact]
    public void AddPackage_With_Empty_Description_Should_Fail()
    {
        var shipment = NewShipment();

        var result = shipment.AddPackage("  ", ValidWeight(), ValidDimensions());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PackageErrors.EmptyDescription);
    }

    [Fact]
    public void AddPackage_With_Negative_DeclaredValue_Should_Fail()
    {
        var shipment = NewShipment();

        var result = shipment.AddPackage(
            "Laptop", ValidWeight(), ValidDimensions(), declaredValue: -1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PackageErrors.InvalidDeclaredValue);
    }

    [Fact]
    public void AddPackage_WhenShipment_Is_No_Longer_Editable_Should_Fail()
    {
        var shipment = DispatchedShipment();

        var result = shipment.AddPackage("New Item", ValidWeight(), ValidDimensions());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.NotEditable);
        shipment.Packages.Should().ContainSingle();
    }

    [Fact]
    public void RemovePackage_WhenEditable_Should_Remove_And_Raise_Event()
    {
        var shipment = NewShipment();
        var packageId = shipment.AddPackage("Laptop", ValidWeight(), ValidDimensions()).Value;

        var result = shipment.RemovePackage(packageId);

        result.IsSuccess.Should().BeTrue();
        shipment.Packages.Should().BeEmpty();
        shipment.DomainEvents.OfType<PackageRemovedFromShipmentEvent>()
            .Should().ContainSingle(e => e.PackageId == packageId);
    }

    [Fact]
    public void RemovePackage_When_Missing_Should_Fail()
    {
        var shipment = NewShipment();

        var result = shipment.RemovePackage(PackageId.New());

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Package.NotFound");
    }

    [Fact]
    public void RemovePackage_When_Dispatched_Should_Fail()
    {
        var shipment = DispatchedShipment();
        var packageId = shipment.Packages[0].Id;

        var result = shipment.RemovePackage(packageId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.NotEditable);
    }

    // ---------------------------------------------------------------
    // Carrier & dispatch
    // ---------------------------------------------------------------

    [Fact]
    public void AssignCarrier_Should_Set_Carrier_And_Estimated_Date()
    {
        var shipment = NewShipment();
        var eta = DateTime.UtcNow.AddDays(2);

        var result = shipment.AssignCarrier("  FedEx  ", eta);

        result.IsSuccess.Should().BeTrue();
        shipment.Tracking.CarrierName.Should().Be("FedEx");
        shipment.Tracking.EstimatedDeliveryDate.Should().Be(eta);
        shipment.DomainEvents.OfType<CarrierAssignedEvent>()
            .Should().ContainSingle(e => e.CarrierName == "FedEx" && e.EstimatedDeliveryDate == eta);
    }

    [Fact]
    public void AssignCarrier_With_Empty_Name_Should_Fail()
    {
        var shipment = NewShipment();

        var result = shipment.AssignCarrier("   ");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.EmptyCarrier);
    }

    [Fact]
    public void AssignCarrier_When_Dispatched_Should_Fail()
    {
        var shipment = DispatchedShipment();

        var result = shipment.AssignCarrier("DHL");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.NotEditable);
    }

    [Fact]
    public void Dispatch_Without_Packages_Should_Fail()
    {
        var shipment = NewShipment();
        shipment.AssignCarrier("FedEx");

        var result = shipment.Dispatch();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.NoPackages);
    }

    [Fact]
    public void Dispatch_Without_Carrier_Should_Fail()
    {
        var shipment = NewShipment();
        shipment.AddPackage("Laptop", ValidWeight(), ValidDimensions());

        var result = shipment.Dispatch();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.EmptyCarrier);
    }

    [Fact]
    public void Dispatch_With_Packages_And_Carrier_Should_Move_To_InTransit()
    {
        var shipment = PreparedShipment();

        var result = shipment.Dispatch();

        result.IsSuccess.Should().BeTrue();
        shipment.Status.Should().Be(ShipmentStatus.InTransit);
        shipment.IsEditable.Should().BeFalse();
        shipment.DomainEvents.OfType<ShipmentDispatchedEvent>()
            .Should().ContainSingle(e => e.CarrierName == "Aramex");
    }

    [Fact]
    public void Dispatch_Twice_Should_Fail()
    {
        var shipment = DispatchedShipment();

        var result = shipment.Dispatch();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.NotEditable);
    }

    // ---------------------------------------------------------------
    // Route points
    // ---------------------------------------------------------------

    [Theory]
    [InlineData(RoutePointType.Transit, ShipmentStatus.InTransit)]
    [InlineData(RoutePointType.SortingFacility, ShipmentStatus.InTransit)]
    [InlineData(RoutePointType.CustomsClearance, ShipmentStatus.AtCustoms)]
    [InlineData(RoutePointType.OutForDelivery, ShipmentStatus.OutForDelivery)]
    public void AddRoutePoint_Should_Update_Status_Based_On_Type(
        RoutePointType type, ShipmentStatus expected)
    {
        var shipment = NewShipment();

        var result = shipment.AddRoutePoint(
            "Cairo Hub", "Processing", DateTime.UtcNow, type);

        result.IsSuccess.Should().BeTrue();
        shipment.Route.Should().ContainSingle();
        shipment.CurrentLocation!.Location.Should().Be("Cairo Hub");
        shipment.Status.Should().Be(expected);
        shipment.DomainEvents.OfType<RoutePointAddedEvent>()
            .Should().ContainSingle(e => e.PointType == type);

        if (type == RoutePointType.OutForDelivery)
        {
            shipment.DomainEvents.OfType<ShipmentOutForDeliveryEvent>()
                .Should().ContainSingle(e => e.CurrentLocation == "Cairo Hub");
        }
    }

    [Fact]
    public void AddRoutePoint_Should_Normalize_Arrival_To_Utc()
    {
        var shipment = NewShipment();
        var local = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Local);

        shipment.AddRoutePoint("Cairo", "Arrived", local, RoutePointType.Transit)
            .IsSuccess.Should().BeTrue();

        shipment.Route[0].ArrivedAt.Kind.Should().Be(DateTimeKind.Utc);
        shipment.Route[0].ArrivedAt.Should().Be(local.ToUniversalTime());
    }

    [Theory]
    [InlineData("", "desc")]
    [InlineData("   ", "desc")]
    [InlineData("Cairo", "")]
    [InlineData("Cairo", "   ")]
    public void AddRoutePoint_With_Empty_Text_Should_Fail(string location, string description)
    {
        var shipment = NewShipment();

        var result = shipment.AddRoutePoint(location, description, DateTime.UtcNow, RoutePointType.Transit);

        result.IsFailure.Should().BeTrue();
        shipment.Route.Should().BeEmpty();
    }

    [Fact]
    public void AddRoutePoint_With_Default_Date_Should_Fail()
    {
        var shipment = NewShipment();

        var result = shipment.AddRoutePoint("Cairo", "Arrived", default, RoutePointType.Transit);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("RoutePoint.InvalidArrivedAt");
    }

    [Fact]
    public void AddRoutePoint_When_Cancelled_Should_Fail()
    {
        var shipment = NewShipment();
        shipment.Cancel("Customer request", "Admin").IsSuccess.Should().BeTrue();

        var result = shipment.AddRoutePoint("Cairo", "Arrived", DateTime.UtcNow, RoutePointType.Transit);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.AlreadyCancelled);
    }

    [Fact]
    public void AddRoutePoint_When_Delivered_Should_Fail()
    {
        var shipment = DispatchedShipment();
        shipment.MarkDelivered(DateTime.UtcNow).IsSuccess.Should().BeTrue();

        var result = shipment.AddRoutePoint("Cairo", "Arrived", DateTime.UtcNow, RoutePointType.Transit);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.InvalidStatusTransition);
    }

    // ---------------------------------------------------------------
    // Delivery
    // ---------------------------------------------------------------

    [Fact]
    public void MarkDelivered_From_Created_Should_Fail()
    {
        var shipment = NewShipment();

        var result = shipment.MarkDelivered(DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.InvalidStatusTransition);
        shipment.Status.Should().Be(ShipmentStatus.Created);
    }

    [Fact]
    public void MarkDelivered_From_InTransit_Should_Deliver_And_Raise_Event()
    {
        var shipment = DispatchedShipment();
        var deliveredAt = DateTime.UtcNow;

        var result = shipment.MarkDelivered(deliveredAt, "  Reception  ");

        result.IsSuccess.Should().BeTrue();
        shipment.Status.Should().Be(ShipmentStatus.Delivered);
        shipment.IsDelivered.Should().BeTrue();
        shipment.DeliveredAt.Should().Be(deliveredAt);
        shipment.DomainEvents.OfType<ShipmentDeliveredEvent>()
            .Should().ContainSingle(e =>
                e.DeliveredAt == deliveredAt && e.ReceivedBy == "Reception");
    }

    [Fact]
    public void MarkDelivered_Twice_Should_Fail()
    {
        var shipment = DispatchedShipment();
        shipment.MarkDelivered(DateTime.UtcNow).IsSuccess.Should().BeTrue();

        var result = shipment.MarkDelivered(DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.AlreadyDelivered);
    }

    [Fact]
    public void MarkDelivered_With_Default_Date_Should_Fail()
    {
        var shipment = DispatchedShipment();

        var result = shipment.MarkDelivered(default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.InvalidDeliveredAt);
    }

    [Fact]
    public void MarkDeliveryFailed_From_Created_Should_Fail()
    {
        var shipment = NewShipment();

        var result = shipment.MarkDeliveryFailed("Nobody home");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.InvalidStatusTransition);
    }

    [Fact]
    public void MarkDeliveryFailed_With_Empty_Reason_Should_Fail()
    {
        var shipment = DispatchedShipment();

        var result = shipment.MarkDeliveryFailed("  ");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.EmptyFailureReason);
    }

    [Fact]
    public void MarkDeliveryFailed_From_InTransit_Should_Mark_Failed()
    {
        var shipment = DispatchedShipment();

        var result = shipment.MarkDeliveryFailed("Nobody home");

        result.IsSuccess.Should().BeTrue();
        shipment.Status.Should().Be(ShipmentStatus.Failed);
        shipment.DomainEvents.OfType<ShipmentDeliveryFailedEvent>()
            .Should().ContainSingle(e => e.FailureReason == "Nobody home");
    }

    [Fact]
    public void MarkDeliveryFailed_When_Already_Delivered_Should_Fail()
    {
        var shipment = DispatchedShipment();
        shipment.MarkDelivered(DateTime.UtcNow).IsSuccess.Should().BeTrue();

        var result = shipment.MarkDeliveryFailed("Late");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.AlreadyDelivered);
    }

    // ---------------------------------------------------------------
    // Cancellation
    // ---------------------------------------------------------------

    [Fact]
    public void Cancel_When_Created_Should_Cancel_And_Record_Reason()
    {
        var shipment = NewShipment();

        var result = shipment.Cancel("Customer requested", "Admin");

        result.IsSuccess.Should().BeTrue();
        shipment.Status.Should().Be(ShipmentStatus.Cancelled);
        shipment.IsCancelled.Should().BeTrue();
        shipment.CancellationReason.Should().Be("Customer requested");
        shipment.CancelledAt.Should().NotBeNull();
        shipment.DomainEvents.OfType<ShipmentCancelledEvent>()
            .Should().ContainSingle(e => e.CancelledBy == "Admin");
    }

    [Fact]
    public void Cancel_With_Empty_Reason_Should_Fail()
    {
        var shipment = NewShipment();

        var result = shipment.Cancel("  ", "Admin");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.EmptyCancelReason);
    }

    [Fact]
    public void Cancel_With_Empty_CancelledBy_Should_Fail()
    {
        var shipment = NewShipment();

        var result = shipment.Cancel("Customer requested", "  ");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.EmptyCancelledBy);
    }

    [Fact]
    public void Cancel_Twice_Should_Fail()
    {
        var shipment = NewShipment();
        shipment.Cancel("Customer requested", "Admin").IsSuccess.Should().BeTrue();

        var result = shipment.Cancel("Again", "Admin");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.AlreadyCancelled);
    }

    [Fact]
    public void Cancel_When_Delivered_Should_Fail()
    {
        var shipment = DispatchedShipment();
        shipment.MarkDelivered(DateTime.UtcNow).IsSuccess.Should().BeTrue();

        var result = shipment.Cancel("No longer needed", "Admin");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.AlreadyDelivered);
    }

    // ---------------------------------------------------------------
    // Address update
    // ---------------------------------------------------------------

    [Fact]
    public void UpdateDeliveryAddress_When_Editable_Should_Replace_Address()
    {
        var shipment = NewShipment();
        var newAddress = DeliveryAddress.Create("New St", "Giza", "Giza Gov", "12345", "Egypt").Value;

        var result = shipment.UpdateDeliveryAddress(newAddress);

        result.IsSuccess.Should().BeTrue();
        shipment.DestinationAddress.Should().Be(newAddress);
        shipment.DomainEvents.OfType<DeliveryAddressUpdatedEvent>().Should().ContainSingle();
    }

    [Fact]
    public void UpdateDeliveryAddress_With_Null_Should_Fail()
    {
        var shipment = NewShipment();

        var result = shipment.UpdateDeliveryAddress(null!);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.InvalidAddress);
    }

    [Fact]
    public void UpdateDeliveryAddress_When_Dispatched_Should_Fail()
    {
        var shipment = DispatchedShipment();
        var newAddress = DeliveryAddress.Create("New St", "Giza", "Giza Gov", "12345", "Egypt").Value;

        var result = shipment.UpdateDeliveryAddress(newAddress);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShipmentErrors.NotEditable);
    }

    // ---------------------------------------------------------------
    // Value objects
    // ---------------------------------------------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Weight_Create_Should_Reject_Non_Positive_Values(decimal value)
    {
        var result = Weight.Create(value, WeightUnit.Kg);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Weight.InvalidValue);
    }

    [Fact]
    public void Weight_ToKilograms_Should_Convert_Units()
    {
        Weight.InGrams(500m).ToKilograms().Should().Be(0.5m);
        Weight.InPounds(1m).ToKilograms().Should().BeApproximately(0.453592m, 0.000001m);
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(1, 0, 1)]
    [InlineData(1, 1, 0)]
    [InlineData(-1, 1, 1)]
    public void Dimensions_Create_Should_Reject_Non_Positive_Sides(decimal l, decimal w, decimal h)
    {
        var result = Dimensions.Create(l, w, h, DimensionUnit.Cm);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Dimensions.Invalid);
    }

    [Theory]
    [InlineData("", "Cairo", "Cairo", "11511", "Egypt")]
    [InlineData("Street", "", "Cairo", "11511", "Egypt")]
    [InlineData("Street", "Cairo", "", "11511", "Egypt")]
    [InlineData("Street", "Cairo", "Cairo", "", "Egypt")]
    [InlineData("Street", "Cairo", "Cairo", "11511", "")]
    public void DeliveryAddress_Create_Should_Return_Failure_Instead_Of_Throwing(
        string street, string city, string state, string zipCode, string country)
    {
        var result = DeliveryAddress.Create(street, city, state, zipCode, country);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DeliveryAddress.Required");
    }

    [Fact]
    public void DeliveryAddress_FullAddress_Should_Include_Apartment_When_Present()
    {
        var withUnit = DeliveryAddress.Create("St", "Cairo", "Cairo", "1", "Egypt", "Apt 4").Value;
        var withoutUnit = DeliveryAddress.Create("St", "Cairo", "Cairo", "1", "Egypt").Value;

        withUnit.FullAddress.Should().Contain("Apt 4");
        withoutUnit.FullAddress.Should().NotContain("Apt");
    }

    [Fact]
    public void TrackingInfo_Create_Should_Return_Failure_For_Empty_Values()
    {
        TrackingInfo.Create("", ShipmentStatus.Created, "desc").IsFailure.Should().BeTrue();
        TrackingInfo.Create("TRK", ShipmentStatus.Created, "").IsFailure.Should().BeTrue();
    }

    [Fact]
    public void RoutePoint_Create_Should_Return_Failure_For_Empty_Values()
    {
        RoutePoint.Create("", "desc", DateTime.UtcNow, RoutePointType.Transit).IsFailure.Should().BeTrue();
        RoutePoint.Create("Cairo", "", DateTime.UtcNow, RoutePointType.Transit).IsFailure.Should().BeTrue();
        RoutePoint.Create("Cairo", "desc", default, RoutePointType.Transit).IsFailure.Should().BeTrue();
    }

    // ---------------------------------------------------------------
    // Identifiers
    // ---------------------------------------------------------------

    [Fact]
    public void ShipmentId_And_PackageId_Should_Behave_Like_Value_Types()
    {
        var shipmentId = ShipmentId.New();
        var packageId = PackageId.New();

        shipmentId.IsEmpty.Should().BeFalse();
        shipmentId.Should().NotBe(ShipmentId.New());
        shipmentId.Should().Be(ShipmentId.From(shipmentId.Value));

        packageId.IsEmpty.Should().BeFalse();
        packageId.Should().NotBe(PackageId.New());

        default(ShipmentId).IsEmpty.Should().BeTrue();
        default(PackageId).IsEmpty.Should().BeTrue();
    }

    // ---------------------------------------------------------------
    // Event contract
    // ---------------------------------------------------------------

    [Fact]
    public void Shipment_Events_Must_Not_Declare_A_Property_Named_Id()
    {
        var eventTypes = typeof(Shipment).Assembly.GetTypes()
            .Where(t => t.Namespace == typeof(ShipmentCreatedEvent).Namespace)
            .Where(t => typeof(DomainEvent).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var eventType in eventTypes)
        {
            var properties = eventType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            Assert.DoesNotContain(properties, p => p.Name == "Id");
        }
    }
}
