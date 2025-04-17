using System;
using Microsoft.Graph;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;

public interface IGraphClient
{
    GraphServiceClient Client { get; }
}
