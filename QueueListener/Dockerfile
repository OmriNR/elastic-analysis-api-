FROM mcr.microsoft.com/dotnet/sdk:11.0-preview AS build
WORKDIR /src

COPY ["QueueListener.sln", "./"]
COPY ["Listener/Listener.csproj", "Listener/"]

RUN dotnet restore

COPY . .
WORKDIR /src/Listener
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:11.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Listener.dll"]
