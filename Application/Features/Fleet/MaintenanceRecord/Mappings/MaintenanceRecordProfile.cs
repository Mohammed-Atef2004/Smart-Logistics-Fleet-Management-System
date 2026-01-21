using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.MaintenanceRecord.Mappings
{
    public class MaintenanceRecordProfile:Profile
    {
        public MaintenanceRecordProfile()
        {
            CreateMap<Domain.Fleet.Entities.MaintenanceRecord, DTOs.MaintenanceRecordDto>().ReverseMap();
            CreateMap<Domain.Fleet.Entities.MaintenanceRecord, DTOs.UpdateMaintenanceRecordDto>().ReverseMap();
        }
    }
}
