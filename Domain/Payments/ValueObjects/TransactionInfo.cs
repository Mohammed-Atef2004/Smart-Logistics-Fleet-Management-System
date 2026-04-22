using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Payments.ValueObjects
{
    public class TransactionInfo
    {
        public string TransactionId { get; }
        public string PaymentGateway { get; }
        public DateTime TransactionDate { get; }
        public TransactionInfo(string transactionId, string paymentGateway, DateTime transactionDate)
        {
            TransactionId = transactionId;
            PaymentGateway = paymentGateway;
            TransactionDate = transactionDate;
        }
    }
}
