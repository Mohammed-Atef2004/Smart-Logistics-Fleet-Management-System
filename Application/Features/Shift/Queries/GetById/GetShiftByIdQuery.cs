using Application.Features.Shift.DTOs;
using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shift.Queries.GetById
{
    public record GetShiftByIdQuery(ShiftId shiftId): IRequest<Result<ShiftDto>>;
    
}
