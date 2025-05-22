namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.DeleteGroup;

[DtoGenerator]
public record DeleteGroupCommand(Guid Id) : IRequest;

public class Validator : AbstractValidator<DeleteGroupCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
