namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CompleteProviderRegistration;

public class CompleteProviderRegistrationCommandHandler(IUserRepository repository, IPubSub pubSub) : IRequestHandler<CompleteProviderRegistrationCommand, Guid>
{
    public async Task<Guid> Handle(CompleteProviderRegistrationCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindByEmailAsync(request.Email, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        aggregate.CompleteProviderRegistration(request.IdentityProviderId);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubSub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);

        return aggregate.Id;
    }
}
