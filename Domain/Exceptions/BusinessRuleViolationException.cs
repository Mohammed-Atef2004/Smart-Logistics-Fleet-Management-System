using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class BusinessRuleViolationException : DomainException
    {
        public IBusinessRule BrokenRule { get; }

        public BusinessRuleViolationException(IBusinessRule brokenRule)
            : base(brokenRule.Message)
        {
            BrokenRule = brokenRule;
        }
    }
}
