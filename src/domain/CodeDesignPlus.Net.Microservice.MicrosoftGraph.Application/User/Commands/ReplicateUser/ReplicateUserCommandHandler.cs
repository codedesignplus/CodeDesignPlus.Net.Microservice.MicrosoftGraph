namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.ReplicateUser;

public class ReplicateUserCommandHandler(IUserRepository repository, IPubSub pubsub) : IRequestHandler<ReplicateUserCommand>
{
    public async Task Handle(ReplicateUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync(request.Email, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.UserAlreadyExists);
        
        var userAggregate = UserAggregate.Create(request.Id, request.IdIdentityProvider, request.IdentityProvider, request.FirstName, request.LastName, request.Email, request.Phone, request.DisplayName, null, null, true, request.IsActive);

        await repository.CreateAsync(userAggregate, cancellationToken);

        await pubsub.PublishAsync(userAggregate.GetAndClearEvents(), cancellationToken);
    }
}