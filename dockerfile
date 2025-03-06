FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY ReceiptSharing.Api/*.csproj ./ReceiptSharing.Api/
RUN dotnet restore ReceiptSharing.Api/ReceiptSharing.Api.csproj

COPY . .
WORKDIR /app/ReceiptSharing.Api
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 5000

CMD ["dotnet", "ReceiptSharing.Api.dll"]
