using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Payments.Enums
{
    public enum PaymentMethod
    {
        CreditCard = 1,
        PayPal = 2,
        BankTransfer = 3,
        Cash = 4
    }
}
