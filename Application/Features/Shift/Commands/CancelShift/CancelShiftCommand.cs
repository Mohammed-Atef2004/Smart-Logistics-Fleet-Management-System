using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shift.Commands.CancelShift
{
    public record CancelShiftCommand(ShiftId ShiftId) : IRequest<Result>;
}
