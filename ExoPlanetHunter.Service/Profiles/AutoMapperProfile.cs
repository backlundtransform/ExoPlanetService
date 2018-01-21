using AutoMapper;

using ExoPlanetHunter.Database.entity;
using ExoPlanetHunter.Service.Dto;

namespace ExoPlanetHunter.Service.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Constellation, ConstellationDto>().ForMember(dest => dest.Stars,
               opts => opts.MapFrom(src => src.Stars.Count));

            CreateMap<Constellation, ConstellationStarsDto>();

            CreateMap<Star, StarDto>().ForMember(dest => dest.Planets, opts => opts.MapFrom(src => src.Planets.Count)).ForMember(dest => dest.Constellation, opts => opts.MapFrom(src => src.Constellation.Name));

            CreateMap<Star, StarPlanetsDto>();
            CreateMap<Planet, PlanetDto>();
        }
    }
}