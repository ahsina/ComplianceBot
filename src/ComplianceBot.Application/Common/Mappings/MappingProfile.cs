using AutoMapper;
using ComplianceBot.Application.Common.DTOs;
using ComplianceBot.Domain.Entities;

namespace ComplianceBot.Application.Common.Mappings;

/// <summary>
/// AutoMapper profile for entity to DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Report mappings
        CreateMap<Report, ReportDto>()
            .ForMember(d => d.SubmittedByUserName, opt => opt.MapFrom(s => s.SubmittedByUser != null ? s.SubmittedByUser.FullName : null));

        // Tenant mappings
        CreateMap<Tenant, TenantDto>();

        // User mappings
        CreateMap<User, UserDto>();

        // Add more mappings as needed
    }
}
