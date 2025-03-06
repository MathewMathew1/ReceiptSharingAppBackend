# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY ReceiptSharing.Api/*.csproj ./ReceiptSharing.Api/
RUN dotnet restore ReceiptSharing.Api/ReceiptSharing.Api.csproj

COPY . . 
WORKDIR /app/ReceiptSharing.Api
RUN dotnet publish -c Release -o /app/out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Install dotnet-ef in the runtime container
RUN dotnet tool install --global dotnet-ef

# Add the tool to PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy app from build container to runtime container
COPY --from=build /app/out ./

EXPOSE 5000

# Run database migration and start app
CMD ["sh", "-c", "dotnet ef database update && dotnet ReceiptSharing.Api.dll"]

