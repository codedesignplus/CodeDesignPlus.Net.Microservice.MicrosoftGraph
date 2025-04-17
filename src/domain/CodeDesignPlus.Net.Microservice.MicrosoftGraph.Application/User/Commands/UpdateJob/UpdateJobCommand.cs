using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.UpdateJob;

[DtoGenerator]
public record UpdateJobCommand(Guid Id, JobInfo Job) : IRequest;

public class Validator : AbstractValidator<UpdateJobCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
