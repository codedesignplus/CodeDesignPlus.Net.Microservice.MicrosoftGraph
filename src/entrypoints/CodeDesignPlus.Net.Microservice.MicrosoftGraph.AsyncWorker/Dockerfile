FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "nuget.config"]
COPY ["src/domain/CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application/CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application.csproj", "domain/CodeDesignPlus.Net.Microservice.MicrosoftGraph.Application/"]
COPY ["src/domain/CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain/CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.csproj", "domain/CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain/"]
COPY ["src/domain/CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure/CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.csproj", "domain/CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure/"]
COPY ["src/entrypoints/CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker/CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.csproj", "entrypoints/CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker/"]
RUN dotnet restore "./entrypoints/CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker/CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.csproj" /p:PublishReadyToRun=true
COPY ["src/domain/", "domain/"]
COPY ["src/entrypoints/CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker/", "entrypoints/CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker/"]

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN dotnet publish "./entrypoints/CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker/CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:PublishReadyToRun=true

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
EXPOSE 5002
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_GCDynamicAdaptationMode=1 \
    DOTNET_GCConserveMemory=5 \
    DOTNET_GCHeapHardLimitPercent=70
USER app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeDesignPlus.Net.Microservice.MicrosoftGraph.AsyncWorker.dll"]
