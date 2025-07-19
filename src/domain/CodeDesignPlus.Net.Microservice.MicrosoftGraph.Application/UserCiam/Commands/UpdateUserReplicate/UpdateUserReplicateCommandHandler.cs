namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.UserCiam.Commands.UpdateUserReplicate;

public class UpdateUserReplicateCommandHandler(IUserCiamRepository repository) : IRequestHandler<UpdateUserReplicateCommand>
{
    public async Task Handle(UpdateUserReplicateCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.FindAsync<UserCiamAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(user, Errors.UserNotFound);

        user.MarkAsReplicated();

        await repository.UpdateAsync(user, cancellationToken);
    }
}