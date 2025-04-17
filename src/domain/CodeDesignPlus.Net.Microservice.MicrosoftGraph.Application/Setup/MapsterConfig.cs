using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.CreateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.UpdateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Setup;

public static class MapsterConfigGraph
{
    public static void Configure() { 

        TypeAdapterConfig<CreateGroupCommand, Domain.Models.Role>.NewConfig();
        TypeAdapterConfig<UpdateGroupCommand, Domain.Models.Role>.NewConfig();

        TypeAdapterConfig<RoleAggregate, RoleDto>.NewConfig();
    }
}
