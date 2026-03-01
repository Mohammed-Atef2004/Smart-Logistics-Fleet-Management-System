using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainServices
{
    public interface ITrackingNumberService
    {
        Task<string> GenerateUniqueAsync(CancellationToken ct = default);
        bool IsValidFormat(string trackingNumber);
    }
}
