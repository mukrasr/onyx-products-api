FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/Onyx.ProductsApi Onyx.ProductsApi
RUN dotnet restore Onyx.ProductsApi

# Unit test runner
FROM build AS unittestrunner
WORKDIR /src

COPY src/Onyx.ProductsApi.UnitTests Onyx.ProductsApi.UnitTests
COPY src/Onyx.ProductsApi Onyx.ProductsApi
RUN dotnet restore Onyx.ProductsApi.UnitTests && \
    dotnet build --no-restore Onyx.ProductsApi.UnitTests
CMD ["dotnet", "test", "Onyx.ProductsApi.UnitTests"]

# Integration test runner
FROM build AS integrationtestrunner
WORKDIR /src

COPY src/Onyx.ProductsApi.IntegrationTests Onyx.ProductsApi.IntegrationTests
COPY scripts/run-integration-tests.sh .
RUN dotnet restore Onyx.ProductsApi.IntegrationTests && \
    dotnet build --no-restore Onyx.ProductsApi.IntegrationTests && \
    chmod +x run-integration-tests.sh
CMD ["./run-integration-tests.sh"]

# Publish
FROM build AS publish
RUN dotnet publish --no-restore Onyx.ProductsApi -c Release -o /app/publish

# Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && \
    apt-get install -y curl
COPY --from=publish /app/publish .
CMD ["dotnet", "Onyx.ProductsApi.dll"]
