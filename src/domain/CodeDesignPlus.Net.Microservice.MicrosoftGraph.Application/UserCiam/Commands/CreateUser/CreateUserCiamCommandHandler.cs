namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.Commands.CreateUser;


public class CreateUserCiamCommandHandler(IUserRepository repository, IPubSub pubsub) : IRequestHandler<CreateUserCiamCommand>
{
    public async Task Handle(CreateUserCiamCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync(request.Email, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.UserAlreadyExists);
        
        var userAggregate = UserCiamAggregate.Create(Guid.NewGuid(), request.FirstName, request.LastName, request.Email, request.Phone, request.DisplayName, true, request.IsActive);

        await repository.CreateAsync(userAggregate, cancellationToken);

        await pubsub.PublishAsync(userAggregate.GetAndClearEvents(), cancellationToken);
    }
}