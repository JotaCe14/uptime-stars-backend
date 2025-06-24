#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER root
RUN apt-get update && apt-get install -y iputils-ping && rm -rf /var/lib/apt/lists/*

#USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Uptime.Stars.Monitor/Uptime.Stars.Monitor.csproj", "Uptime.Stars.Monitor/"]
COPY ["Uptime.Stars.Infrastructure/Uptime.Stars.Infrastructure.csproj", "Uptime.Stars.Infrastructure/"]
COPY ["Uptime.Stars.Application/Uptime.Stars.Application.csproj", "Uptime.Stars.Application/"]
COPY ["Uptime.Stars.Contracts/Uptime.Stars.Contracts.csproj", "Uptime.Stars.Contracts/"]
COPY ["Uptime.Stars.Domain/Uptime.Stars.Domain.csproj", "Uptime.Stars.Domain/"]
COPY ["Uptime.Stars.Persistence/Uptime.Stars.Persistence.csproj", "Uptime.Stars.Persistence/"]
RUN dotnet restore "./Uptime.Stars.Monitor/Uptime.Stars.Monitor.csproj"
COPY . .
WORKDIR "/src/Uptime.Stars.Monitor"
RUN dotnet build "./Uptime.Stars.Monitor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Uptime.Stars.Monitor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Uptime.Stars.Monitor.dll"]