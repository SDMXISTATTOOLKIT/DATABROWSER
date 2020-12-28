#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
COPY . /app
WORKDIR /app
RUN dotnet restore "test/DataBrowser.IntegrationTests/DataBrowser.IntegrationTests.csproj"