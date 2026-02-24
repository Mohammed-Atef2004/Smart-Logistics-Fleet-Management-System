using Application.Features.Shift.DTOs;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shift.Queries.GetAll
{
    public record GetAllShiftsQuery: IRequest<Result<List<ShiftDto>>>;

}
