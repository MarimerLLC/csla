FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY AppServer2/AppServer2.csproj AppServer2/
COPY BusinessLibrary/BusinessLibrary.csproj BusinessLibrary/
RUN dotnet restore AppServer2/AppServer2.csproj
COPY . .
WORKDIR /src/AppServer2
RUN dotnet build AppServer2.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish AppServer2.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AppServer2.dll"]
