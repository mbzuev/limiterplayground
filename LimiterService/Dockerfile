FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LimiterService/LimiterService.csproj", "LimiterService/"]
RUN dotnet restore "LimiterService/LimiterService.csproj"
COPY . .
WORKDIR "/src/LimiterService"
RUN dotnet build "LimiterService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LimiterService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LimiterService.dll"]
