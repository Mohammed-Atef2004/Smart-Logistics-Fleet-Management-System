using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.Errors
{
    public class AddressErrors
    {
        public static Error EmptyStreet = 
            new("Address.EmptyStreet", "Street cannot be empty.");
        public static Error EmptyCity
            = new("Address.EmptyCity", "City cannot be empty.");
        public static Error EmptyCountry 
            = new("Address.EmptyCountry", "Country cannot be empty.");
        public static Error EmptyZipCode 
            = new("Address.EmptyZipCode", "Zip code cannot be empty.");
    }
}
