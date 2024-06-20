FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY AppServer1/AppServer1.csproj AppServer1/
COPY BusinessLibrary/BusinessLibrary.csproj BusinessLibrary/
RUN dotnet restore AppServer1/AppServer1.csproj
COPY . .
WORKDIR /src/AppServer1
RUN dotnet build AppServer1.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish AppServer1.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AppServer1.dll"]
