using AutoMapper;
using LittleFootStockManager.Data.Model;
using LittleFootStockManager.Dto.User;
using System.Diagnostics.Metrics;

namespace LittleFootStockManager.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            //User
            CreateMap<Users, UserDto>().ReverseMap();
        }
    }
}
