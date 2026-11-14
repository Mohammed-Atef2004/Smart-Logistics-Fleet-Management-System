// Layer-wide usings for the Application project.
// Declared once here instead of repeating the same using block
// at the top of every handler, validator, and mapping file.
// Persistence (Microsoft.EntityFrameworkCore) is deliberately NOT
// globalized: handlers owning persistence concerns is Phase 3 debt.

global using Application.Common.Interfaces;
global using AutoMapper;
global using Domain.SharedKernel;
global using FluentValidation;
global using MediatR;
global using Microsoft.Extensions.Logging;
