using AutoMapper;
using Application.Features.Fleet.Vehicle.Commands.Create;
using Application.Features.Fleet.Vehicle.DTOs;
using Domain.Vehicles;

public class VehicleMappingProfile : Profile
{
    public  VehicleMappingProfile()
    {
        
        CreateMap<Vehicle, VehicleDto>().ReverseMap();

        CreateMap<UpdateVechicleDto, Vehicle>().ReverseMap();

    }
}