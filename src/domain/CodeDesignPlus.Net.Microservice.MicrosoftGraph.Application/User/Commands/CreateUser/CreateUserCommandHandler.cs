namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;

public class CreateUserCommandHandler(IUserRepository repository) : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.UserAlreadyExists);

        var user = UserAggregate.Create(request.Id);

        await repository.CreateAsync(user, cancellationToken);
    }
}