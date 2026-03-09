using AutoMapper;
using SwitchManagment.API.Models.Dto.Switch.Request;
using SwitchManagment.API.Models.Dto.Switch.Response;
//using SwitchManagment.API.Repository.Entities;
using SwitchManagment.API.Db.Entities;
using SwitchManagment.API.Models.Dto.Switch.Request.Get;
using SwitchManagment.API.Models.Dto.Switch.Response.Get;
using SwitchManagment.API.Models.Dto.Switch.Response.Admin;

namespace SwitchManagment.API.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<SwitchEntity, SwitchResponse>();
            CreateMap<SwitchEntity, AdminSwitchResponse>();
            CreateMap<AdminSwitchCreateRequest, SwitchEntity>();
            CreateMap<GetRequest, GetResponse>();
            CreateMap<SortRequest, SortResponse>();
            CreateMap<PageNavRequest, PageNavResponse>();
        }
    }
}
