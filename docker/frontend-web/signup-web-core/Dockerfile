FROM mcr.microsoft.com/dotnet/core/sdk:3.1.402 as builder

WORKDIR /src
COPY src/SignUp.Core/SignUp.Core.csproj ./SignUp.Core/
COPY src/SignUp.Entities/SignUp.Entities.csproj ./SignUp.Entities/
COPY src/SignUp.Messaging/SignUp.Messaging.csproj ./SignUp.Messaging/
COPY src/SignUp.Web.Core/SignUp.Web.Core.csproj ./SignUp.Web.Core/

WORKDIR /src/SignUp.Web.Core
RUN dotnet restore

COPY src /src
RUN dotnet publish -c Release -o /out SignUp.Web.Core.csproj

# app image
FROM  mcr.microsoft.com/dotnet/core/aspnet:3.1.8

EXPOSE 80
ENTRYPOINT ["dotnet", "/app/SignUp.Web.Core.dll"]
ENV ReferenceDataApi:Url=http://reference-data-api/api

WORKDIR /app
COPY --from=builder /out/ .