FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BattleGame.Shared/BattleGame.Shared.csproj  BattleGame.Shared/
COPY BattleGame.LoadBalancer/BattleGame.LoadBalancer.csproj  BattleGame.LoadBalancer/

RUN dotnet restore BattleGame.LoadBalancer/BattleGame.LoadBalancer.csproj

COPY BattleGame.Shared/  BattleGame.Shared/
COPY BattleGame.LoadBalancer/  BattleGame.LoadBalancer/

RUN dotnet publish BattleGame.LoadBalancer/BattleGame.LoadBalancer.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 9000
ENTRYPOINT ["dotnet", "BattleGame.LoadBalancer.dll"]