# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:11.0-preview AS build
WORKDIR /src

# 1. Copy the solution file if it's in the root
COPY ["StoreAPI.sln", "./"]

# 2. Copy project files using the correct folder paths
# Assuming your projects are inside the 'StoreAPI' folder based on your screenshot
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
# Also copy the Common library if WebAPI depends on it

# 3. Restore based on the WebAPI path
RUN dotnet restore "WebAPI/WebAPI.csproj"

# 4. Copy the entire context
COPY . .

# 5. Move to the directory where the WebAPI project lives to publish
WORKDIR "/src/StoreAPI/WebAPI"
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish

# Stage 2: runtime image
FROM mcr.microsoft.com/dotnet/aspnet:11.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "WebAPI.dll"]