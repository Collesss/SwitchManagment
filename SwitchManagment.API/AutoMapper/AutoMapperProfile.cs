using AutoMapper;
//using SwitchManagment.API.Repository.Entities;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Models.Dto.Switch.Port;
using SwitchManagment.API.Models.Dto.Switch.Request;
using SwitchManagment.API.Models.Dto.Switch.Request.Get;
using SwitchManagment.API.Models.Dto.Switch.Response;
using SwitchManagment.API.Models.Dto.Switch.Response.Admin;
using SwitchManagment.API.Models.Dto.Switch.Response.Get;
using SwitchManagment.API.Models.Dto.Switch.Response.Port;
using SwitchManagment.API.SwitchService.Data;

namespace SwitchManagment.API.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<SwitchEntity, SwitchResponse>();
            CreateMap<SwitchEntity, SwitchWithPortsResponse>();
            CreateMap<SwitchEntity, AdminSwitchWithPortsResponse>();
            CreateMap<SwitchEntity, AdminSwitchResponse>();
            CreateMap<AdminSwitchCreateRequest, SwitchEntity>();
            CreateMap<GetRequest, GetResponse>();
            CreateMap<SortRequest, SortResponse>();
            CreateMap<PageNavRequest, PageNavResponse>();
            
            CreateMap<SwitchPort, PortResponse>();
            CreateMap<SwitchPortType, PortTypeResponse>();
            CreateMap<SwitchPortStatus, PortStatusResponse>();

            CreateMap<SwitchVlan, VlanResponse>();

            CreateMap<SwitchEntity, PortConfigAccess>();
            CreateMap<InterfaceEntity, PortConfigAccess>()
                .ForMember(pca => pca.InterfaceName, opt => opt.MapFrom(i => i.Name));
            CreateMap<ConfigurePortAccessRequest, PortConfigAccess>()
                .ForMember(pca => pca.AccessVlan, opt => opt.MapFrom(pcar => pcar.Vlan));
        }
    }
}
