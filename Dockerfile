FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/Onyx.ProductsApi Onyx.ProductsApi
COPY src/Onyx.ProductsApi.Contracts Onyx.ProductsApi.Contracts
RUN dotnet restore Onyx.ProductsApi

# Test runner
FROM build AS testrunner
WORKDIR /src

COPY src/Onyx.ProductsApi.Tests Onyx.ProductsApi.Tests
COPY src/Onyx.ProductsApi.Contracts Onyx.ProductsApi.Contracts
RUN dotnet restore Onyx.ProductsApi.Tests && \
    dotnet build --no-restore Onyx.ProductsApi.Tests
CMD ["dotnet", "test", "Onyx.ProductsApi.Tests"]

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
