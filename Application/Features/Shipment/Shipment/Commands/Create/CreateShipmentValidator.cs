using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Features.Shipment.Shipment.Commands.Create
{
    public class CreateShipmentValidator:AbstractValidator<CreateShipmentCommand>
    {
        public CreateShipmentValidator() 
        {
         RuleFor(s=>s.shipmentDto.TrackingNumber).NotEmpty().MaximumLength(20);
        }
    }
}
