using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.CreateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.UpdateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.DataTransferObjects;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Setup;

public static class MapsterConfigGraph
{
    public static void Configure() { 

        
        TypeAdapterConfig<ContactInfo, Domain.Models.ContactInfo>.NewConfig();
        TypeAdapterConfig<JobInfo, Domain.Models.JobInfo>.NewConfig();

        TypeAdapterConfig<CreateGroupCommand, Domain.Models.Role>.NewConfig();
        TypeAdapterConfig<UpdateGroupCommand, Domain.Models.Role>.NewConfig();

        
        TypeAdapterConfig<UpdateIdentityCommand, Domain.Models.User>.NewConfig();

        TypeAdapterConfig<RoleAggregate, RoleDto>.NewConfig();
    }
}
