namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;

[DtoGenerator]
/// <param name="Id">El usuario.</param>
/// <param name="Role">El id del grupo del proveedor de identidad, no el nombre del rol.</param>
public record AddGroupToUserCommand(Guid Id, Guid Role) : IRequest;

public class Validator : AbstractValidator<AddGroupToUserCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Role).NotEmpty().NotNull();
    }
}
