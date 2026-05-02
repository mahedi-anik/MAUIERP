using AutoMapper;
using MAUIERP.ApplicationLayer.DTOs.MasterData;
using MAUIERP.Domain.Entities.MasterData;
using MAUIERP.Domain.Enums;

namespace MAUIERP.ApplicationLayer.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Company mappings
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.BranchCount, opt => opt.MapFrom(src => src.Branches.Count(b => !b.IsDeleted)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateCompanyDto, Company>()
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<UpdateCompanyDto, Company>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // Branch mappings
            CreateMap<Branch, BranchDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateBranchDto, Branch>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BranchStatus.Active));
        }
    }
}