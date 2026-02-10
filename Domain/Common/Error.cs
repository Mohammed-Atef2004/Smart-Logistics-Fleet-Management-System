using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public record Error(string Code, string Message)
    {
        public static readonly Error None = new("", "");
    }

}
