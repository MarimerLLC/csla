FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["ProjectTracker.AppServerCore/ProjectTracker.AppServerCore.csproj", "ProjectTracker.AppServerCore/"]
COPY ["ProjectTracker.DalMock/ProjectTracker.DalMock.csproj", "ProjectTracker.DalMock/"]
COPY ["ProjectTracker.Dal/ProjectTracker.Dal.csproj", "ProjectTracker.Dal/"]
COPY ["ProjectTracker.BusinessLibrary.Netstandard/ProjectTracker.BusinessLibrary.Netstandard.csproj", "ProjectTracker.BusinessLibrary.Netstandard/"]
RUN dotnet restore "ProjectTracker.AppServerCore/ProjectTracker.AppServerCore.csproj"
COPY . .
WORKDIR "/src/ProjectTracker.AppServerCore"
RUN dotnet build "ProjectTracker.AppServerCore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ProjectTracker.AppServerCore.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ProjectTracker.AppServerCore.dll"]