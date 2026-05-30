namespace Domain.Payments.Enums
{
    public enum PaymentStatus
    {
        Pending,    // اتعمل ولسه ما اتعالجش
        Processed,  // اتعالج بنجاح
        Failed,     // فشل
        Refunded    // اترجع
    }
}
