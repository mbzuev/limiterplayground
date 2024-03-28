FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LimitedStorage/LimitedStorage.csproj", "LimitedStorage/"]
RUN dotnet restore "LimitedStorage/LimitedStorage.csproj"
COPY . .
WORKDIR "/src/LimitedStorage"
RUN dotnet build "LimitedStorage.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LimitedStorage.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LimitedStorage.dll"]
