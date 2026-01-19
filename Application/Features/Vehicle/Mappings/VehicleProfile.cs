using AutoMapper;
using Domain.Fleet;
using Application.Features.Vehicles.DTOs;
using Application.Features.Vehicle.Commands.CreateVehicle;

public class VehicleMappingProfile : Profile
{
    public  VehicleMappingProfile()
    {
        
        CreateMap<Vehicle, VehicleDto>().ReverseMap();

        CreateMap<UpdateVechicleDto, Vehicle>().ReverseMap();

    }
}