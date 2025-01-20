FROM mcr.microsoft.com/dotnet/sdk:8.0 AS format-check
WORKDIR /app
COPY . .
RUN dotnet format --verify-no-changes

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ["ChessApi/ChessApi.csproj", "ChessApi/"]
RUN dotnet restore "ChessApi/ChessApi.csproj"
COPY . .
RUN dotnet build "ChessApi/ChessApi.csproj" -c Release -o /app/build

FROM build AS test
COPY ["ChessApi.Tests/ChessApi.Tests.csproj", "ChessApi.Tests/"]
RUN dotnet restore "ChessApi.Tests/ChessApi.Tests.csproj"
COPY ["ChessApi.Tests/*", "ChessApi.Tests/"]
RUN dotnet test "ChessApi.Tests/ChessApi.Tests.csproj"

FROM build AS migrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT dotnet ef database update --project ChessApi -- --environment Staging

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS backend
WORKDIR /app
COPY --from=build /app/build .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ChessApi.dll"]
