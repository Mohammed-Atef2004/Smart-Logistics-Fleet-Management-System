using Domain.Invoices;
using Domain.Invoices.Enums;
using Domain.Invoices.Events;
using Domain.Invoices.ValueObjects;
using Domain.SharedKernel;

public class Invoice : AggregateRoot<InvoiceId>
{
    private readonly List<InvoiceItem> _items = new();

    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

    public decimal TotalPrice => _items.Sum(x => x.Price);

    public InvoiceStatus Status { get; private set; }

    private Invoice() { }

    private Invoice(InvoiceId id)
    {
        Id = id;
        Status = InvoiceStatus.Draft;
    }

    public static Result<Invoice> Create()
    {
        var invoice = new Invoice(InvoiceId.New());

        invoice.AddDomainEvent(new InvoiceCreatedEvent(invoice.Id,invoice.TotalPrice));

        return Result<Invoice>.Success(invoice);
    }

    // ------------------------
    // Behavior
    // ------------------------

    public Result AddItem(string description, decimal price, int quantity)
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(new Error("Invoice.Paid", "Cannot add items unless invoice is draft"));

        if (price <= 0)
            return Result.Failure(new Error("Invoice.Paid", "Price must be positive"));

        if (quantity <= 0)
            return Result.Failure(new Error("Invoice.Paid", "Quantity must be positive"));

        var item = new InvoiceItem(description, price, quantity);

        _items.Add(item);

        return Result.Success();
    }

    public Result Issue()
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(InvoiceErrors.NotDraft);

        if (!_items.Any())
            return Result.Failure(InvoiceErrors.Empty);

        Status = InvoiceStatus.Issued;

        AddDomainEvent(new InvoiceIssuedEvent(Id, TotalPrice));

        return Result.Success();
    }

    public Result Pay()
    {
        if (Status != InvoiceStatus.Issued)
            return Result.Failure(InvoiceErrors.NotIssued);

        Status = InvoiceStatus.Paid;

        AddDomainEvent(new InvoicePaidEvent(Id));

        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == InvoiceStatus.Paid)
            return Result.Failure(new Error("Invoice.Paid","Cannot cancel a paid invoice"));

        Status = InvoiceStatus.Cancelled;

        AddDomainEvent(new InvoiceCancelledEvent(Id));

        return Result.Success();
    }
}