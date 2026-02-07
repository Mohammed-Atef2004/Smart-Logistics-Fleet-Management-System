using Domain.Common;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Claims
{
    public class InsuranceClaim : BaseEntity
    {
        public Guid ShipmentId { get; private set; }
        public Money ClaimAmount { get; private set; }
        public string Reason { get; private set; }

        private InsuranceClaim() { }

        public InsuranceClaim(Guid shipmentId, Money claimAmount, string reason)
        {
            ShipmentId = shipmentId;
            ClaimAmount = claimAmount;
            Reason = reason;
        }
    }
}
