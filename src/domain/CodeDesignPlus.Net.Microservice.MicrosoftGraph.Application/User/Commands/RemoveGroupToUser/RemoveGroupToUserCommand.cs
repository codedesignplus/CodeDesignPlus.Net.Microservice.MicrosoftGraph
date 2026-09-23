namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;

[DtoGenerator]
/// <param name="Id">El usuario.</param>
/// <param name="Role">El id del grupo del proveedor de identidad, no el nombre del rol.</param>
public record RemoveGroupToUserCommand(Guid Id, Guid Role) : IRequest;

public class Validator : AbstractValidator<RemoveGroupToUserCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Role).NotEmpty().NotNull();
    }
}
