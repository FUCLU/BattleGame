FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BattleGame.Shared/BattleGame.Shared.csproj BattleGame.Shared/
COPY BattleGame.Server/BattleGame.Server.csproj BattleGame.Server/

RUN dotnet restore BattleGame.Server/BattleGame.Server.csproj

COPY BattleGame.Shared/ BattleGame.Shared/
COPY BattleGame.Server/ BattleGame.Server/
COPY BattleGame.Client/Config/ BattleGame.Client/Config/

RUN dotnet publish BattleGame.Server/BattleGame.Server.csproj -c Release -o /app/publish --no-restore

# Fail image build early if character config is missing in publish output.
RUN test -d /app/publish/Config/Characters \
 && find /app/publish/Config/Characters -maxdepth 1 -name '*.json' | grep -q .

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
WORKDIR /app

RUN mkdir -p /app/logs

COPY --from=build /app/publish .

EXPOSE 9001
ENTRYPOINT ["dotnet", "BattleGame.Server.dll"]
