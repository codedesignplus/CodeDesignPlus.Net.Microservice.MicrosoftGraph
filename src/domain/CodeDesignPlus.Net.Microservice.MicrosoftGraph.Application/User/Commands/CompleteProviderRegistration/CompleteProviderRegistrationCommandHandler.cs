using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CompleteProviderRegistration;

public class CompleteProviderRegistrationCommandHandler(IUserRepository repository, IPubSub pubSub, ILogger<CompleteProviderRegistrationCommandHandler> logger) : IRequestHandler<CompleteProviderRegistrationCommand, Guid>
{
    public async Task<Guid> Handle(CompleteProviderRegistrationCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "CompleteProviderRegistration started. Email: {Email} | IdentityProviderId: {IdentityProviderId}",
            request.Email, request.IdentityProviderId);

        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        logger.LogDebug("CompleteProviderRegistration: looking up user by email. Email: {Email}", request.Email);

        var aggregate = await repository.FindByEmailAsync(request.Email, cancellationToken);

        logger.LogDebug(
            "CompleteProviderRegistration: FindByEmailAsync result. Email: {Email} | Found: {Found}",
            request.Email, aggregate is not null);

        ApplicationGuard.IsNull(aggregate, Errors.UserNotFound);

        logger.LogDebug(
            "CompleteProviderRegistration: calling CompleteProviderRegistration on aggregate. AggregateId: {AggregateId} | IdentityProviderId: {IdentityProviderId}",
            aggregate!.Id, request.IdentityProviderId);

        aggregate.CompleteProviderRegistration(request.IdentityProviderId);

        await repository.UpdateAsync(aggregate, cancellationToken);

        logger.LogDebug("CompleteProviderRegistration: aggregate updated in repository. AggregateId: {AggregateId}", aggregate.Id);

        await pubSub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);

        logger.LogInformation(
            "CompleteProviderRegistration completed. AggregateId: {AggregateId} | Email: {Email} | IdentityProviderId: {IdentityProviderId}",
            aggregate.Id, request.Email, request.IdentityProviderId);

        return aggregate.Id;
    }
}
