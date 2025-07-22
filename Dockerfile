FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY src/RouteSearchApi/*.csproj ./src/RouteSearchApi/
COPY src/RouteSearchApiTests/*.csproj ./src/RouteSearchApiTests/
RUN dotnet restore

COPY . .
RUN dotnet publish src/RouteSearchApi/RouteSearchApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "RouteSearchApi.dll"]
