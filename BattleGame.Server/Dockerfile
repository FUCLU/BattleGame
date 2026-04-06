# ── STAGE 1: BUILD ───────────────────────────────────────────
# Dùng .NET SDK để biên dịch source code
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Bước 1: Copy file .csproj trước
# → Docker cache layer này, chỉ restore lại khi .csproj thay đổi
# → Giúp build nhanh hơn ở những lần sau
COPY BattleGame.Shared/BattleGame.Shared.csproj  BattleGame.Shared/
COPY BattleGame.Server/BattleGame.Server.csproj  BattleGame.Server/

# Bước 2: Tải NuGet packages
RUN dotnet restore BattleGame.Server/BattleGame.Server.csproj

# Bước 3: Copy toàn bộ source code của 2 project
# (Shared vì Server phụ thuộc vào nó)
COPY BattleGame.Shared/  BattleGame.Shared/
COPY BattleGame.Server/  BattleGame.Server/

# Bước 4: Build và xuất file chạy vào /app/publish
RUN dotnet publish BattleGame.Server/BattleGame.Server.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── STAGE 2: RUNTIME ─────────────────────────────────────────
# Chỉ dùng .NET Runtime (nhỏ hơn SDK ~3 lần), không có compiler
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
WORKDIR /app

# Tạo thư mục chứa log server
RUN mkdir -p /app/logs

# Copy artifact đã build từ stage 1 sang
COPY --from=build /app/publish .

# Khai báo port server lắng nghe (Client kết nối vào đây)
EXPOSE 9001

# Lệnh chạy khi container khởi động
ENTRYPOINT ["dotnet", "BattleGame.Server.dll"]