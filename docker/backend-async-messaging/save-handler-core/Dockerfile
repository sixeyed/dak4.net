FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100 as builder

WORKDIR /src
COPY src/SignUp.Core/SignUp.Core.csproj ./SignUp.Core/
COPY src/SignUp.Entities/SignUp.Entities.csproj ./SignUp.Entities/
COPY src/SignUp.Messaging/SignUp.Messaging.csproj ./SignUp.Messaging/
COPY src/SignUp.MessageHandlers.SaveProspectCore/SignUp.MessageHandlers.SaveProspectCore.csproj ./SignUp.MessageHandlers.SaveProspectCore/

WORKDIR /src/SignUp.MessageHandlers.SaveProspectCore
RUN dotnet restore

COPY src /src
RUN dotnet publish -c Release -o /out SignUp.MessageHandlers.SaveProspectCore.csproj

# app image
FROM mcr.microsoft.com/dotnet/core/runtime:3.0

WORKDIR /save-prospect-handler
ENTRYPOINT ["dotnet", "SignUp.MessageHandlers.SaveProspectCore.dll"]

COPY --from=builder /out/ .