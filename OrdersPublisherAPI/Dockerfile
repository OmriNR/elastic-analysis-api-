# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:11.0-preview AS build
WORKDIR /src

# Copy project files
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]

# Restore based on the WebAPI path
RUN dotnet restore "WebAPI/WebAPI.csproj"

# Copy the entire context
COPY . .

# Move to the directory where the WebAPI project lives to publish
WORKDIR "/src/WebAPI"
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish

# Stage 2: runtime image
FROM mcr.microsoft.com/dotnet/aspnet:11.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "WebAPI.dll"]