using AutoMapper;
using DataEntities;

namespace DomainEntities.MapperConfig
{
    /// <summary>
    /// configuration file for mapping entity classes to dto classes
    /// </summary>
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<News, NewsDto>().ReverseMap();
        }
    }
}
