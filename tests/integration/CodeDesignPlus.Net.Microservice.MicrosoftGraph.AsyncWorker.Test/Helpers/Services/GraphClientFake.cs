using System;
using CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;
using Microsoft.Graph;
using Moq;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.Test.Helpers.Services;

public class GraphClientFake : IGraphClient
{
    public GraphServiceClient Client { get; private set; } = (GraphServiceClient)Mock.Of<IBaseClient>();
}
