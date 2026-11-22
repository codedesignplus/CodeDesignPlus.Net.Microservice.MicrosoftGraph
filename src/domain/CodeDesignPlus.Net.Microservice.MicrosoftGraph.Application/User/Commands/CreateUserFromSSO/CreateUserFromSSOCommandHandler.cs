namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.User.Commands.CreateUserFromSSO;

public class CreateUserFromSSOCommandHandler(IUserRepository repository, IOptions<UserDocumentOptions> documentOptions) : IRequestHandler<CreateUserFromSSOCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserFromSSOCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync(request.Email, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.UserAlreadyExists);

        var userId = Guid.NewGuid();

        var aggregate = UserAggregate.CreateFromSSO(userId, request.FirstName, request.LastName, request.Email, request.Phone, request.DisplayName, request.DocumentNumber, request.DocumentType, request.IsActive, documentOptions.Value.ToRules());

        await repository.CreateAsync(aggregate, cancellationToken);

        return userId;
    }
}
