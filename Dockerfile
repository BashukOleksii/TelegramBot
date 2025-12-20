FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копіюємо файл проекту та відновлюємо залежності (кешується окремо)
COPY ["TelegramBot_MinimalAPI.csproj", "./"]
RUN dotnet restore

# Тепер копіюємо все інше і збираємо
COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Змушуємо додаток працювати на порту 8000
ENV ASPNETCORE_URLS=http://+:8000
EXPOSE 8000

ENTRYPOINT ["dotnet", "TelegramBot_MinimalAPI.dll"]
