using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.Errors
{
    public class PackageErrors
    {
        public static readonly Error EmptyDescription = new(
            "Package.EmptyDescription",
            "Package description cannot be empty.");
        public static readonly Error InvalidWeight = new(
            "Package.InvalidWeight",
            "Package weight must be greater than zero.");
        public static readonly Error InvalidDimensions = new(
            "Package.InvalidDimensions",
            "Package dimensions must be greater than zero.");
        public static readonly Error InvalidDeclaredValue = new(
            "Package.InvalidDeclaredValue",
            "Declared value cannot be negative.");
        public static readonly Error EmptyCurrency = new(
            "Package.EmptyCurrency",
            "Currency cannot be empty.");
    }
}
