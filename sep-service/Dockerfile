#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM registry.access.redhat.com/ubi8/dotnet-31 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["sep-service.csproj", ""]
RUN dotnet restore "./sep-service.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "sep-service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "sep-service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "sep-service.dll"]