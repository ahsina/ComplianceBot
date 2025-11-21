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
        CreateMap<Report, ReportDto>()
            .ForMember(d => d.SubmittedByUserName, opt => opt.MapFrom(s => s.SubmittedByUser != null ? s.SubmittedByUser.FullName : null));

        // Add more mappings as needed
    }
}
