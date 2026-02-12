using AutoMapper;
using SaaS.Application.DTOs;
using SaaS.Application.Mappings;
using SaaS.Domain.Entities;
using Xunit;

namespace SaaS.Api.Tests.Mappings;

public class MappingTests
{
    [Fact]
    public void Configuration_IsValid()
    {
        var configuration = new MapperConfiguration(cfg => {
             cfg.AddProfile<MappingProfile>();
        });
        
        configuration.AssertConfigurationIsValid();
    }
}
