# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the repository so project references can be resolved.
COPY . .

# Restore and publish the backend project.
RUN dotnet publish backend/FastPCB.API -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy only the published output into the final image.
COPY --from=build /app/out .

# Railway injects PORT at runtime; 8080 is the local/container fallback.
EXPOSE 8080

ENTRYPOINT ["dotnet", "FastPCB.API.dll"]
CMD ["--urls", "http://0.0.0.0:8080"]
