#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR .
COPY ["src/WSHUB/WSHUB.csproj", "src/WSHUB/"]
COPY ["src/DataBrowser.AC.Modules/DataBrowser.AC.Modules.csproj", "src/DataBrowser.AC.Modules/"]
COPY ["src/DataBrowser.UseCase/DataBrowser.UseCase.csproj", "src/DataBrowser.UseCase/"]
COPY ["src/DataBrowser.Query/DataBrowser.Query.csproj", "src/DataBrowser.Query/"]
COPY ["src/DataBrowser.Domain/DataBrowser.Domain.csproj", "src/DataBrowser.Domain/"]
COPY ["src/DataBrowser.Specifications/DataBrowser.Specifications.csproj", "src/DataBrowser.Specifications/"]
COPY ["src/DataBrowser.AC/DataBrowser.AC.csproj", "src/DataBrowser.AC/"]
COPY ["src/EndPointConnector.Interfaces.Sdmx/EndPointConnector.Interfaces.Sdmx.csproj", "src/EndPointConnector.Interfaces.Sdmx/"]
COPY ["src/EndPointConnector.Interfaces/EndPointConnector.Interfaces.csproj", "src/EndPointConnector.Interfaces/"]
COPY ["src/EndPointConnector.Models/EndPointConnector.Models.csproj", "src/EndPointConnector.Models/"]
COPY ["src/EndPointConnector.Interfaces.Spod/EndPointConnector.Interfaces.Spod.csproj", "src/EndPointConnector.Interfaces.Spod/"]
COPY ["src/DataBrowser.Interfaces/DataBrowser.Interfaces.csproj", "src/DataBrowser.Interfaces/"]
COPY ["src/TracertLOg/TracertLOg.csproj", "src/TracertLOg/"]
COPY ["src/DataBrowser.Subcribers/DataBrowser.Subcribers.csproj", "src/DataBrowser.Subcribers/"]
COPY ["src/DataBrowser.Command/DataBrowser.Command.csproj", "src/DataBrowser.Command/"]
COPY ["src/DataBrowser.INF.Modules/DataBrowser.INF.Modules.csproj", "src/DataBrowser.INF.Modules/"]
COPY ["src/DataBrowser.INF.SdmxCache/DataBrowser.INF.SdmxCache.csproj", "src/DataBrowser.INF.SdmxCache/"]
COPY ["src/EndPointConnector.ParserSdmx/EndPointConnector.ParserSdmx.csproj", "src/EndPointConnector.ParserSdmx/"]
COPY ["src/DataBrowser.DB.Dapper/DataBrowser.DB.Dapper.csproj", "src/DataBrowser.DB.Dapper/"]
COPY ["src/Sister.EndPointConnector.Sdmx/Sister.EndPointConnector.Sdmx.csproj", "src/Sister.EndPointConnector.Sdmx/"]
COPY ["src/Sister.EndPointConnector.Spod/Sister.EndPointConnector.Spod.csproj", "src/Sister.EndPointConnector.Spod/"]
COPY ["src/EndPointConnector.JsonStatParser/EndPointConnector.JsonStatParser.csproj", "src/EndPointConnector.JsonStatParser/"]
COPY ["src/DataBrowser.DB.EFCore/DataBrowser.DB.EFCore.csproj", "src/DataBrowser.DB.EFCore/"]
COPY ["src/DataBrowser.INF.NodeWS.File/DataBrowser.INF.NodeWS.File.csproj", "src/DataBrowser.INF.NodeWS.File/"]
RUN dotnet restore "WSHUB/WSHUB.csproj"
COPY . .
WORKDIR "WSHUB"
RUN dotnet build "WSHUB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WSHUB.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WSHUB.dll"]
