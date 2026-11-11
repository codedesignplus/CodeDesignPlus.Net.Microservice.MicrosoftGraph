using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.DomainEvents.Catalogs;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.Repositories;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Consumers;

/// <summary>
/// Los eventos de ms-catalogs llenan la copia local de tipos de documento (pendings/663).
/// </summary>
public class TypeDocumentProjectionHandlersTest
{
    private readonly Mock<ITypeDocumentRepository> repository = new();
    private readonly Guid id = Guid.NewGuid();

    [Fact]
    public async Task Created_StoresAnActiveCopy()
    {
        // Act
        await new TypeDocumentCreatedProjectionHandler(repository.Object).HandleAsync(TypeDocumentCreatedDomainEvent.Create(id, "Cédula de Extranjería", null, "CE", true), CancellationToken.None);

        // Assert
        repository.Verify(x => x.ApplyAsync(It.Is<TypeDocumentAggregate>(t => t.Id == id && t.Code == "CE" && t.IsActive && !t.IsDeleted), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Updated_Inactive_StoresAnInactiveCopy()
    {
        // Act
        await new TypeDocumentUpdatedProjectionHandler(repository.Object).HandleAsync(TypeDocumentUpdatedDomainEvent.Create(id, "Tarjeta de Identidad", null, "TI", false), CancellationToken.None);

        // Assert
        repository.Verify(x => x.ApplyAsync(It.Is<TypeDocumentAggregate>(t => t.Id == id && t.Name == "Tarjeta de Identidad" && !t.IsActive), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deleted_StoresADeletedCopy()
    {
        // Act
        await new TypeDocumentDeletedProjectionHandler(repository.Object).HandleAsync(TypeDocumentDeletedDomainEvent.Create(id, "Registro Civil", null, "RC", false), CancellationToken.None);

        // Assert
        repository.Verify(x => x.ApplyAsync(It.Is<TypeDocumentAggregate>(t => t.Id == id && t.IsDeleted && !t.IsActive), It.IsAny<CancellationToken>()), Times.Once);
    }
}
