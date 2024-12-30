FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

WORKDIR /app

COPY . .

# Run migrations
FROM base AS migrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT dotnet ef database update --project ChessApi -- --environment Staging
