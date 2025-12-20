# Етап збірки (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TelegramBot_MinimalAPI.csproj", "./"]
RUN dotnet restore "TelegramBot_MinimalAPI.csproj"
COPY . .
RUN dotnet publish "TelegramBot_MinimalAPI.csproj" -c Release -o /app/publish
# Етап запуску (Runtime)
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TelegramBot_MinimalAPI.dll"]