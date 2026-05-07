using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Invoices.Commands.Create
{
    public class CreateInoivceCommandValidator:AbstractValidator<CreateInvoiceCommand>
    {
        public CreateInoivceCommandValidator()
        {
          RuleFor(x => x).NotNull().WithMessage("Command cannot be null");
          RuleFor(x => x).Custom((command, context) =>
          {
              if (command == null)
                  return;
          });
         
        }
    }
}
