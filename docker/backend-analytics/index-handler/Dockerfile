FROM mcr.microsoft.com/dotnet/core/sdk:3.1.402 as builder

WORKDIR /src
COPY src/SignUp.Core/SignUp.Core.csproj ./SignUp.Core/
COPY src/SignUp.Entities/SignUp.Entities.csproj ./SignUp.Entities/
COPY src/SignUp.Messaging/SignUp.Messaging.csproj ./SignUp.Messaging/
COPY src/SignUp.MessageHandlers.IndexProspect/SignUp.MessageHandlers.IndexProspect.csproj ./SignUp.MessageHandlers.IndexProspect/

WORKDIR /src/SignUp.MessageHandlers.IndexProspect
RUN dotnet restore

COPY src /src
RUN dotnet publish -c Release -o /out SignUp.MessageHandlers.IndexProspect.csproj

# app image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1.8

WORKDIR /index-prospect-handler
ENTRYPOINT ["dotnet", "SignUp.MessageHandlers.IndexProspect.dll"]

COPY --from=builder /out/ .