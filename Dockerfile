FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev

WORKDIR /app

COPY . .
