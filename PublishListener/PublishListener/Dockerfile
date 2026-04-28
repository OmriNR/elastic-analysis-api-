FROM mcr.microsoft.com/dotnet/runtime:11.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PublishListener/PublishListener.csproj", "PublishListener/"]
RUN dotnet restore "PublishListener/PublishListener.csproj"
COPY . .
WORKDIR "/src/PublishListener"
RUN dotnet build "./PublishListener.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PublishListener.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PublishListener.dll"]
