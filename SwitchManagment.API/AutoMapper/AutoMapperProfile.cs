using AutoMapper;
using SwitchManagment.API.Models.Dto.Switch;
using SwitchManagment.API.Repository.Entities;

namespace SwitchManagment.API.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<SwitchEntity, SwitchResponse>();
            CreateMap<SwitchCreateRequest, SwitchEntity>();
        }
    }
}
