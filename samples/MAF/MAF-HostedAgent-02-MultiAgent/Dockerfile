FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["MAF-HostedAgent-02-MultiAgent.csproj", "."]
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
EXPOSE 8088

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MAF-HostedAgent-02-MultiAgent.dll"]
