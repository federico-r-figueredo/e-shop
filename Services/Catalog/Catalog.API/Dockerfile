FROM mcr.microsoft.com/dotnet/sdk:8.0

ARG BUILD_CONFIGURATION=Debug
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
EXPOSE 5003
EXPOSE 7273

WORKDIR /Source

COPY ["/BuildingBlocks/EventBus/EventBus/EventBus.csproj", "/Source/BuildingBlocks/EventBus/EventBus/EventBus.csproj"]
COPY ["/BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "/Source/BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj"]
COPY ["/BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj", "/Source/BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj"]
COPY ["/Services/Catalog/Catalog.API/Catalog.API.csproj", "/Source/Services/Catalog/Catalog.API/Catalog.API.csproj"]

RUN dotnet restore Services/Catalog/Catalog.API/Catalog.API.csproj
COPY . .
WORKDIR "/Source/Services/Catalog/Catalog.API"
# TODO: Fix this command. Failing w/ "error MSB4018: The 'ResolvePackageAssets' task failed unexpectedly"
# RUN dotnet build --no-restore
RUN dotnet build
RUN dotnet dev-certs https --trust

ENTRYPOINT [ "dotnet", "run", "--no-build" ]