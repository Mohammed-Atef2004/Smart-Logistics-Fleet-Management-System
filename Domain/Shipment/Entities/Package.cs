using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Entities
{
    public class Package : BaseEntity
    {
        public double Weight { get; private set; }
        public string Description { get; private set; }

        private Package() { }

        public Package(double weight, string description)
        {
            Weight = weight;
            Description = description;
        }
    }

}
