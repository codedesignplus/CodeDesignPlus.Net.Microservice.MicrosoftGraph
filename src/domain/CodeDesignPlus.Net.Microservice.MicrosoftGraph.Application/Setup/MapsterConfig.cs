using CodeDesignPlus.Microservice.Api.Dtos;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.CreateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.DeleteGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.UpdateGroup;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.DataTransferObjects;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.DeleteUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.ReplicateUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateContactInfo;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateIdentity;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateJob;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateProfile;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.Commands.CreateUser;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Setup;

public static class MapsterConfigGraph
{
    public static void Configure()
    {


        TypeAdapterConfig<ContactInfo, Domain.Models.ContactInfo>.NewConfig();
        TypeAdapterConfig<JobInfo, Domain.Models.JobInfo>.NewConfig();

        TypeAdapterConfig<CreateGroupCommand, Domain.Models.Role>.NewConfig();
        TypeAdapterConfig<UpdateGroupCommand, Domain.Models.Role>.NewConfig();

        TypeAdapterConfig<UpdateIdentityCommand, Domain.Models.User>.NewConfig();
        TypeAdapterConfig<CreateUserCommand, Domain.Models.User>.NewConfig();

        TypeAdapterConfig<RoleAggregate, RoleDto>.NewConfig();

        TypeAdapterConfig<CreateUserDto, CreateUserCommand>.NewConfig();
        TypeAdapterConfig<ReplicateUserDto, ReplicateUserCommand>.NewConfig();
        TypeAdapterConfig<UpdateIdentityDto, UpdateIdentityCommand>.NewConfig();
        TypeAdapterConfig<UpdateContactInfoDto, UpdateContactInfoCommand>.NewConfig();
        TypeAdapterConfig<UpdateJobDto, UpdateJobCommand>.NewConfig();
        TypeAdapterConfig<UpdateProfileDto, UpdateProfileCommand>.NewConfig();
        TypeAdapterConfig<DeleteUserDto, DeleteUserCommand>.NewConfig();

        TypeAdapterConfig<AddGroupToUserDto, AddGroupToUserCommand>.NewConfig();
        TypeAdapterConfig<RemoveGroupToUserDto, RemoveGroupToUserCommand>.NewConfig();

        TypeAdapterConfig<CreateGroupDto, CreateGroupCommand>.NewConfig();
        TypeAdapterConfig<UpdateGroupDto, UpdateGroupCommand>.NewConfig();
        TypeAdapterConfig<DeleteGroupDto, DeleteGroupCommand>.NewConfig();

        TypeAdapterConfig<CreateUserCiamDto, CreateUserCiamCommand>.NewConfig();
    }
}
