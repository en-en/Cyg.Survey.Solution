#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Cyg.Survey.Solution/src/01-Presentation/Cyg.Application.Api/Cyg.Application.Api.csproj", "Cyg.Application.Api/"]
RUN dotnet restore "Cyg.Application.Api/Cyg.Application.Api.csproj"
COPY . .
WORKDIR "/src/Cyg.Application.Api"
RUN dotnet build "Cyg.Application.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cyg.Application.Api.csproj" -c Release -o /app/publish
COPY ["Cyg.Application.Api/bin/Cyg.Application.Api.xml","/app/publish/"]
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cyg.Application.Api.dll"]