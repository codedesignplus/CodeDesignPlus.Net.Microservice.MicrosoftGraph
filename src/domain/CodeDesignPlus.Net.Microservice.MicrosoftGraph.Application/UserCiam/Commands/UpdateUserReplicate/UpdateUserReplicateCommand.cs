namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.Commands.UpdateUserReplicate;

[DtoGenerator]
public record UpdateUserReplicateCommand(Guid Id) : IRequest;

public class Validator : AbstractValidator<UpdateUserReplicateCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
