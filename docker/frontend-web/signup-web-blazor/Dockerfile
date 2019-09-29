FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100 as builder

WORKDIR /src
COPY src/SignUp.Core/SignUp.Core.csproj ./SignUp.Core/
COPY src/SignUp.Entities/SignUp.Entities.csproj ./SignUp.Entities/
COPY src/SignUp.Messaging/SignUp.Messaging.csproj ./SignUp.Messaging/
COPY src/SignUp.Web.Blazor/SignUp.Web.Blazor.csproj ./SignUp.Web.Blazor/

WORKDIR /src/SignUp.Web.Blazor
RUN dotnet restore

COPY src /src
RUN dotnet publish -c Release -o /out SignUp.Web.Blazor.csproj

# app image
FROM  mcr.microsoft.com/dotnet/core/aspnet:3.0

EXPOSE 80
WORKDIR /signup-web-blazor
ENTRYPOINT ["dotnet", "SignUp.Web.Blazor.dll"]

COPY --from=builder /out/ .