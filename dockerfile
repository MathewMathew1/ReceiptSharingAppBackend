# Use official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set working directory
WORKDIR /app

# Copy project files
COPY . . 

# Install Entity Framework CLI
RUN dotnet tool install --global dotnet-ef

# Set PATH for EF tools to work
ENV PATH="$PATH:/root/.dotnet/tools"

# Restore dependencies
RUN dotnet restore

# Run migrations before starting the app
RUN dotnet ef database update  # ✅ This ensures the database schema is up to date

# Build and publish
RUN dotnet publish -c Release -o out

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/out .

# Expose port and start app
CMD ["dotnet", "ReceiptSharing.Api.dll"]

