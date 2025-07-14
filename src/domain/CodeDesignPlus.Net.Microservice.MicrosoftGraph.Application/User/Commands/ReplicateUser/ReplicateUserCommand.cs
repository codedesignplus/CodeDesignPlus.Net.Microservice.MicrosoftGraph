namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.ReplicateUser;

[DtoGenerator]
public record ReplicateUserCommand(Guid Id,string FirstName, string LastName, string Email, string Phone, string? DisplayName, bool IsActive) : IRequest;

public class Validator : AbstractValidator<ReplicateUserCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.FirstName).NotEmpty().NotNull();
        RuleFor(x => x.LastName).NotEmpty().NotNull();
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty().NotNull();
    }
}
