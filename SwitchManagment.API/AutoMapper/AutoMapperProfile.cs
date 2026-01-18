using AutoMapper;
using SwitchManagment.API.Models.Dto.Switch.Request;
using SwitchManagment.API.Models.Dto.Switch.Response;
//using SwitchManagment.API.Repository.Entities;
using SwitchManagment.API.Db.Entities;

namespace SwitchManagment.API.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<SwitchEntity, SwitchAnnotationResponse>();
            CreateMap<SwitchCreateRequest, SwitchEntity>();
        }
    }
}
