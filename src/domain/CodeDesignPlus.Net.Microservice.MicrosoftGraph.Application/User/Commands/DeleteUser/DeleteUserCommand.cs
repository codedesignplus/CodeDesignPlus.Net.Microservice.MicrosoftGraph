namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.DeleteUser;

[DtoGenerator]
public record DeleteUserCommand(Guid Id) : IRequest;

public class Validator : AbstractValidator<DeleteUserCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
