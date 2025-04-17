namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.Role.Commands.UpdateGroup;

[DtoGenerator]
public record UpdateGroupCommand(Guid Id, string Name, string Description, bool IsActive) : IRequest;

public class Validator : AbstractValidator<UpdateGroupCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
