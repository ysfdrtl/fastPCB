# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Build context should be the repository root so project references can be restored safely.
COPY backend/FastPCB.API/FastPCB.API.csproj backend/FastPCB.API/
COPY backend/FastPCB.Services/FastPCB.Services.csproj backend/FastPCB.Services/
COPY backend/FastPCB.Data/FastPCB.Data.csproj backend/FastPCB.Data/
COPY backend/FastPCB.Models/FastPCB.Models.csproj backend/FastPCB.Models/
RUN dotnet restore backend/FastPCB.API/FastPCB.API.csproj

COPY . .
RUN dotnet publish backend/FastPCB.API/FastPCB.API.csproj -c Release -o out --no-restore /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && groupadd --system fastpcb \
    && useradd --system --gid fastpcb --home-dir /app fastpcb \
    && mkdir -p /app/uploads/projects \
    && chown -R fastpcb:fastpcb /app

COPY --from=build --chown=fastpcb:fastpcb /app/out .

EXPOSE 8080
USER fastpcb

ENTRYPOINT ["sh", "-c", "dotnet FastPCB.API.dll --urls http://0.0.0.0:${PORT:-8080}"]
