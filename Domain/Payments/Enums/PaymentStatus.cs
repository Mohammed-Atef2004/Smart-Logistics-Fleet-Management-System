namespace Domain.Payments.Enums
{
    public enum PaymentStatus
    {
        Pending,    // Created but not processed yet
        Processed,  // Processed successfully
        Failed,     // Payment failed
        Refunded    // Money returned to the payer
    }
}
