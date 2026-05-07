using Domain.Invoices;
using Domain.Invoices.Enums;
using Domain.SharedKernel;
using FluentAssertions;

namespace Domain.UnitTests.Invoices;

public class InvoiceTests
{
    // ------------------------
    // Create
    // ------------------------

    [Fact]
    public void Create_Should_Create_Draft_Invoice()
    {
        // Act
        var result = Invoice.Create();

        // Assert
        result.IsSuccess.Should().BeTrue();

        var invoice = result.Value;

        invoice.Should().NotBeNull();
        invoice.Status.Should().Be(InvoiceStatus.Draft);
        invoice.Items.Should().BeEmpty();
        invoice.TotalPrice.Should().Be(0);
    }

    // ------------------------
    // AddItem
    // ------------------------

    [Fact]
    public void AddItem_Should_Add_Item_When_Invoice_Is_Draft()
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        // Act
        var result = invoice.AddItem(
            "Laptop",
            1000,
            2);

        // Assert
        result.IsSuccess.Should().BeTrue();

        invoice.Items.Should().HaveCount(1);

        var item = invoice.Items.First();

        item.Description.Should().Be("Laptop");
        item.Price.Should().Be(1000);
        item.Quantity.Should().Be(2);
    }

    [Fact]
    public void AddItem_Should_Return_Failure_When_Status_Is_Not_Draft()
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        invoice.AddItem("Item", 100, 1);
        invoice.Issue();

        // Act
        var result = invoice.AddItem(
            "Another Item",
            200,
            1);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void AddItem_Should_Return_Failure_When_Price_Is_Invalid(decimal price)
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        // Act
        var result = invoice.AddItem(
            "Laptop",
            price,
            1);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_Should_Return_Failure_When_Quantity_Is_Invalid(int quantity)
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        // Act
        var result = invoice.AddItem(
            "Laptop",
            100,
            quantity);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void TotalPrice_Should_Be_Calculated_Correctly()
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        invoice.AddItem("Item 1", 100, 2);
        invoice.AddItem("Item 2", 50, 1);

        // Act
        var total = invoice.TotalPrice;

        // Assert
        total.Should().Be(150);
    }

    // ------------------------
    // Issue
    // ------------------------

    [Fact]
    public void Issue_Should_Change_Status_To_Issued()
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        invoice.AddItem("Laptop", 1000, 1);

        // Act
        var result = invoice.Issue();

        // Assert
        result.IsSuccess.Should().BeTrue();

        invoice.Status.Should().Be(InvoiceStatus.Issued);
    }

    [Fact]
    public void Issue_Should_Return_Failure_When_Invoice_Is_Empty()
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        // Act
        var result = invoice.Issue();

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Issue_Should_Return_Failure_When_Status_Is_Not_Draft()
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        invoice.AddItem("Laptop", 1000, 1);
        invoice.Issue();

        // Act
        var result = invoice.Issue();

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    // ------------------------
    // Pay
    // ------------------------

    [Fact]
    public void Pay_Should_Change_Status_To_Paid()
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        invoice.AddItem("Laptop", 1000, 1);
        invoice.Issue();

        // Act
        var result = invoice.Pay();

        // Assert
        result.IsSuccess.Should().BeTrue();

        invoice.Status.Should().Be(InvoiceStatus.Paid);
    }

    [Fact]
    public void Pay_Should_Return_Failure_When_Invoice_Is_Not_Issued()
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        // Act
        var result = invoice.Pay();

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    // ------------------------
    // Cancel
    // ------------------------

    [Fact]
    public void Cancel_Should_Change_Status_To_Cancelled()
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        invoice.AddItem("Laptop", 1000, 1);

        // Act
        var result = invoice.Cancel();

        // Assert
        result.IsSuccess.Should().BeTrue();

        invoice.Status.Should().Be(InvoiceStatus.Cancelled);
    }

    [Fact]
    public void Cancel_Should_Return_Failure_When_Invoice_Is_Paid()
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        invoice.AddItem("Laptop", 1000, 1);
        invoice.Issue();
        invoice.Pay();

        // Act
        var result = invoice.Cancel();

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    // ------------------------
    // State Transition Flow
    // ------------------------

    [Fact]
    public void Invoice_Should_Follow_Valid_Lifecycle()
    {
        // Arrange
        var invoice = Invoice.Create().Value;

        // Draft
        invoice.Status.Should().Be(InvoiceStatus.Draft);

        // Add Item
        invoice.AddItem("Laptop", 1000, 1);

        // Issue
        var issueResult = invoice.Issue();

        issueResult.IsSuccess.Should().BeTrue();
        invoice.Status.Should().Be(InvoiceStatus.Issued);

        // Pay
        var payResult = invoice.Pay();

        payResult.IsSuccess.Should().BeTrue();
        invoice.Status.Should().Be(InvoiceStatus.Paid);
    }
}