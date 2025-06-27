# Base image with .NET 9 runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# 👇 Install OCR tools before switching user
USER root
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        poppler-utils \
        tesseract-ocr && \
    rm -rf /var/lib/apt/lists/*

# 🔁 Switch back to app user (Visual Studio sets this at runtime)
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["SquadflowAI.API/SquadflowAI.API.csproj", "SquadflowAI.API/"]
RUN dotnet restore "./SquadflowAI.API/SquadflowAI.API.csproj"

COPY . .
WORKDIR "/src/SquadflowAI.API"
RUN dotnet build "./SquadflowAI.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SquadflowAI.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: production container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SquadflowAI.API.dll"]
