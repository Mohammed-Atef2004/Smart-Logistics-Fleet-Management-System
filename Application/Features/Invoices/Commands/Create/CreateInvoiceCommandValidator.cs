namespace Application.Features.Invoices.Commands.Create
{
    public class CreateInvoiceCommandValidator:AbstractValidator<CreateInvoiceCommand>
    {
        public CreateInvoiceCommandValidator()
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