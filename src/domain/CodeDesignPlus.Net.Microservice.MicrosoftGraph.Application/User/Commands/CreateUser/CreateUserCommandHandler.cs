using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUser;

public class CreateUserCommandHandler(IUserRepository repository, IMapper mapper, IIdentityServer identityServer, IPubSub pubSub) : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.UserAlreadyExists);

        var user = UserAggregate.Create(request.Id, request.FirstName, request.LastName, request.Email, request.Phone, request.DisplayName, request.Password, request.IsActive);

        await repository.CreateAsync(user, cancellationToken);

        await identityServer.CreateUserAsync(mapper.Map<Domain.Models.User>(request), cancellationToken);

        await pubSub.PublishAsync(user.GetAndClearEvents(), cancellationToken);
    }
}