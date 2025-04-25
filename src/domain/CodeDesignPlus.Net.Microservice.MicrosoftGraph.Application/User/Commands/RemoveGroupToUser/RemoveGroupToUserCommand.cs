namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.RemoveGroupToUser;

[DtoGenerator]
public record RemoveGroupToUserCommand(Guid Id, string Role) : IRequest;

public class Validator : AbstractValidator<RemoveGroupToUserCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Role).NotEmpty().NotNull();
    }
}
