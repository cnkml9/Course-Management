#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/AuthService/AuthService.API/AuthService.API.csproj", "src/Services/AuthService/AuthService.API/"]
COPY ["src/Services/AuthService/Infrastructure/AuthService.Infrastructure/AuthService.Infrastructure.csproj", "src/Services/AuthService/Infrastructure/AuthService.Infrastructure/"]
COPY ["src/Services/AuthService/Core/AuthService.Application/AuthService.Application.csproj", "src/Services/AuthService/Core/AuthService.Application/"]
COPY ["src/Services/AuthService/Core/AuthService.Domain/AuthService.Domain.csproj", "src/Services/AuthService/Core/AuthService.Domain/"]
COPY ["src/Services/AuthService/Infrastructure/AuthService.Persistence/AuthService.Persistence.csproj", "src/Services/AuthService/Infrastructure/AuthService.Persistence/"]
RUN dotnet restore "src/Services/AuthService/AuthService.API/AuthService.API.csproj"
COPY . .
WORKDIR "/src/src/Services/AuthService/AuthService.API"
RUN dotnet build "AuthService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.API.dll"]