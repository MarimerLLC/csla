#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BlazorExample/Server/BlazorExample.Server.csproj", "BlazorExample/Server/"]
COPY ["BlazorExample/Client/BlazorExample.Client.csproj", "BlazorExample/Client/"]
COPY ["BlazorExample/Shared/BlazorExample.Shared.csproj", "BlazorExample/Shared/"]
COPY ["BlazorExample/DataAccess.Mock/DataAccess.Mock.csproj", "BlazorExample/DataAccess.Mock/"]
COPY ["BlazorExample/DataAccess/DataAccess.csproj", "BlazorExample/DataAccess/"]
COPY ["BlazorExample/DataAccess.EF/DataAccess.EF.csproj", "BlazorExample/DataAccess.EF/"]
RUN dotnet restore "BlazorExample/Server/BlazorExample.Server.csproj"
COPY . .
WORKDIR "/src/BlazorExample/Server"
RUN dotnet build "BlazorExample.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorExample.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorExample.Server.dll"]