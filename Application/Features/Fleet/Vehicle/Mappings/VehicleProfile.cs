using AutoMapper;
using Application.Features.Fleet.Vehicle.Commands.Create;
using Domain.Fleet.Entities;
using Application.Features.Fleet.Vehicle.DTOs;

public class VehicleMappingProfile : Profile
{
    public  VehicleMappingProfile()
    {
        
        CreateMap<Vehicle, VehicleDto>().ReverseMap();

        CreateMap<UpdateVechicleDto, Vehicle>().ReverseMap();

    }
}