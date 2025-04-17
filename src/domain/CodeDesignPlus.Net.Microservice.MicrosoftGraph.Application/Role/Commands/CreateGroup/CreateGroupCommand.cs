namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.CreateGroup;

[DtoGenerator]
public record CreateGroupCommand(Guid Id, string Name, string Description, bool IsActive) : IRequest;

public class Validator : AbstractValidator<CreateGroupCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
