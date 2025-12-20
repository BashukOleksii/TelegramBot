# 1. Беремо образ з .NET SDK (для збірки)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# 2. Робоча папка всередині контейнера
WORKDIR /app

# 3. Копіюємо ВЕСЬ проєкт у контейнер
COPY . .

# 4. Збираємо і публікуємо застосунок
RUN dotnet publish -c Release -o out

# 5. Беремо легкий runtime образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# 6. Робоча папка для запуску
WORKDIR /app

# 7. Копіюємо зібраний застосунок
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:8000
EXPOSE 8000

# 9. Команда запуску програми
ENTRYPOINT ["dotnet", "TelegramBot_MinimalAPI.dll"]



