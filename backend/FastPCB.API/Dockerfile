FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Cozum ve proje dosyalarini kopyalayip once restore yap.
COPY ["fastPCB.sln", "./"]
COPY ["fastPCB/backend/FastPCB.API/FastPCB.API.csproj", "fastPCB/backend/FastPCB.API/"]
COPY ["fastPCB/backend/FastPCB.Services/FastPCB.Services.csproj", "fastPCB/backend/FastPCB.Services/"]
COPY ["fastPCB/backend/FastPCB.Data/FastPCB.Data.csproj", "fastPCB/backend/FastPCB.Data/"]
COPY ["fastPCB/backend/FastPCB.Models/FastPCB.Models.csproj", "fastPCB/backend/FastPCB.Models/"]
RUN dotnet restore "fastPCB/backend/FastPCB.API/FastPCB.API.csproj"

# Kaynak kodlari kopyalayip publish et.
COPY . .
WORKDIR /src/fastPCB/backend/FastPCB.API
RUN dotnet publish "FastPCB.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FastPCB.API.dll"]
