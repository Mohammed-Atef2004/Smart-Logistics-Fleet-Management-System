using Domain.Payments.ValueObjects;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Payments.Events
{
    public record PaymentProcessedEvent
        (PaymentId Value):DomainEvent;
}
