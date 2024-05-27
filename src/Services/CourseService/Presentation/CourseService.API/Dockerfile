#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/CourseService/Presentation/CourseService.API/CourseService.API.csproj", "src/Services/CourseService/Presentation/CourseService.API/"]
COPY ["src/Services/AuthService/Core/AuthService.Application/AuthService.Application.csproj", "src/Services/AuthService/Core/AuthService.Application/"]
COPY ["src/Services/AuthService/Core/AuthService.Domain/AuthService.Domain.csproj", "src/Services/AuthService/Core/AuthService.Domain/"]
COPY ["src/Services/CourseService/Infrastructure/CourseService.Infrastructure/CourseService.Infrastructure.csproj", "src/Services/CourseService/Infrastructure/CourseService.Infrastructure/"]
COPY ["src/Services/CourseService/Core/CourseService.Application/CourseService.Application.csproj", "src/Services/CourseService/Core/CourseService.Application/"]
COPY ["src/Services/CourseService/Core/CourseService.Domain/CourseService.Domain.csproj", "src/Services/CourseService/Core/CourseService.Domain/"]
RUN dotnet restore "src/Services/CourseService/Presentation/CourseService.API/CourseService.API.csproj"
COPY . .
WORKDIR "/src/src/Services/CourseService/Presentation/CourseService.API"
RUN dotnet build "CourseService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CourseService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CourseService.API.dll"]