namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.AddGroupToUser;

[DtoGenerator]
public record AddGroupToUserCommand(Guid Id, string Role) : IRequest;

public class Validator : AbstractValidator<AddGroupToUserCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Role).NotEmpty().NotNull();
    }
}
