# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy all project files
COPY . .

# Restore and publish
RUN dotnet publish backend/FastPCB.API -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/out .

# Expose port (Railway will use $PORT env var)
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "FastPCB.API.dll"]
CMD ["--urls", "http://0.0.0.0:8080"]
