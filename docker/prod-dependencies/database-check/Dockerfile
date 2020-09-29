FROM mcr.microsoft.com/dotnet/core/sdk:3.1.402 as builder

WORKDIR /src
COPY src/Utilities.DatabaseCheck/Utilities.DatabaseCheck.csproj ./Utilities.DatabaseCheck/

WORKDIR /src/Utilities.DatabaseCheck
RUN dotnet restore

COPY src /src
RUN dotnet publish -c Release -o /out Utilities.DatabaseCheck.csproj

# app image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1.8

WORKDIR /database-check
ENTRYPOINT ["dotnet", "Utilities.DatabaseCheck.dll"]

COPY --from=builder /out/ .