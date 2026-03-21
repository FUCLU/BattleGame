# NT106.Q23.ANTT — LẬP TRÌNH MẠNG CĂN BẢN

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Windows Forms](https://img.shields.io/badge/Windows_Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL_15-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

## I. Đồ Án Môn Học

**Tên đề tài:** Thiết kế và xây dựng game đối kháng 2D bằng C# trên nền tảng .NET Windows Forms

---

## Danh Sách Thành Viên

| STT | Họ và tên | MSSV | Vai trò |
|---|---|---|---|
| 1 | Lưu Hồng Phúc | 24521382 | Game Architect / Server |
| 2 | Phan Thái Hưng | 24520624 | Gameplay Programmer |
| 3 | Nguyễn Tấn Phát | 24521306 | UI / UX Game Developer |
| 4 | Nguyễn Phan Hoàng Long | 24521006 | Network Programmer |

---

## Mô Tả Tổng Quan

BattleGame là game đối kháng 2D theo thời gian thực qua mạng cục bộ (LAN), được xây dựng bằng C# và Windows Forms. Hai người chơi kết nối tới server trung tâm, chọn nhân vật, được ghép cặp tự động và thi đấu thông qua giao tiếp TCP Socket.

### Các tính năng chính

- Đăng ký tài khoản với xác thực OTP qua Email (SMTP)
- Đăng nhập / xác thực tài khoản người chơi (BCrypt)
- Quên mật khẩu / đặt lại mật khẩu qua OTP Email
- Chọn nhân vật với các chỉ số và kỹ năng riêng biệt
- Hệ thống matchmaking tự động ghép 2 người chơi vào cùng một phòng
- Trận đấu real-time: di chuyển, tấn công, sử dụng kỹ năng
- Đồng bộ trạng thái game liên tục giữa server và các client (50ms/tick)
- Hiển thị animation nhân vật và hiệu ứng chiến đấu
- Hệ thống âm thanh BGM và SFX

### Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ lập trình | C# (.NET 8) |
| Giao diện | Windows Forms |
| Giao tiếp mạng | TCP Socket (`System.Net.Sockets`) |
| Serialization | Custom `PacketSerializer` |
| Cơ sở dữ liệu | PostgreSQL 15 |
| Xác thực mật khẩu | BCrypt (`BCrypt.Net-Next`) |
| Gửi Email OTP | SMTP Gmail / Mailpit (dev local) |
| Container | Docker + Docker Compose |
| Kiểm thử | xUnit (`BattleGame.Test`) |

---

## Yêu Cầu Hệ Thống

| Công cụ | Phiên bản | Ghi chú |
|---|---|---|
| Docker Desktop | 4.x trở lên | Bắt buộc để chạy server + DB + Mailpit |
| .NET SDK | 8.0 | Chỉ cần nếu chạy Client hoặc build tay |
| Visual Studio | 2022 | Để phát triển |
| Windows | 10/11 64-bit | Để chạy Client WinForms |

---

## Hướng Dẫn Cài Đặt & Chạy

### 1. Clone dự án

```bash
git clone https://github.com/FUCLU/BattleGame.git
cd BattleGameSolution
```

### 2. Tạo file .env

> File `.env` chứa mật khẩu DB và không được commit lên Git.
> Phải tạo thủ công sau khi clone.

```bash
cp .env.example .env
```

Mở file `.env` và điền thông tin:

```env
# PostgreSQL
POSTGRES_DB=battlegame
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_password_here

# Server
SERVER_PORT=9000

# SMTP — dev local dùng Mailpit (giữ nguyên, không cần sửa)
SMTP_HOST=mailpit
SMTP_PORT=1025
SMTP_USERNAME=test@battlegame.local
SMTP_PASSWORD=
SMTP_ENABLE_SSL=false
```

### 3. Cài NuGet packages (chỉ làm 1 lần)

```bash
dotnet restore
```

Nếu vẫn báo thiếu package, cài thủ công:

```bash
dotnet add BattleGame.Server/BattleGame.Server.csproj package Npgsql
dotnet add BattleGame.Server/BattleGame.Server.csproj package BCrypt.Net-Next
dotnet add BattleGame.Server/BattleGame.Server.csproj package Microsoft.Extensions.Configuration.Json
dotnet add BattleGame.Server/BattleGame.Server.csproj package Microsoft.Extensions.Configuration.EnvironmentVariables
dotnet add BattleGame.Server/BattleGame.Server.csproj package Microsoft.Extensions.Configuration.Binder
```

### 4. Chạy Server + DB bằng Docker

```bash
# Lần đầu hoặc sau khi thay đổi code
docker compose up --build

# Các lần sau (không đổi code)
docker compose up -d
```

Sau khi chạy:
- **Game Server** → `localhost:9000`
- **PostgreSQL** → `localhost:5433`
- **Mailpit Web UI** → http://localhost:8025 (xem email OTP)
- **Mailpit SMTP** → `localhost:1025`

Kiểm tra log:
```bash
docker compose logs server     # log game server
docker compose logs db         # log database
docker compose logs mailpit    # log email
```

Dừng server:
```bash
docker compose down            # dừng, giữ data
docker compose down -v         # dừng và xóa toàn bộ data DB
```

### 5. Chạy Client (WinForms)

> Client **không** chạy trong Docker. Chạy trực tiếp trên máy Windows.

**Cách 1 — Visual Studio:**
1. Mở `BattleGameSolution.slnx`
2. Chuột phải `BattleGame.Client` → **Set as Startup Project**
3. Nhấn **F5**

**Cách 2 — Terminal:**
```bash
cd BattleGame.Client
dotnet run
```

> Mở 2 cửa sổ Client để thử nghiệm matchmaking.
> Client mặc định kết nối `127.0.0.1:9000`.
> Nếu server chạy máy khác, sửa địa chỉ trong `Config/ClientConfig.cs`.

### 6. Chạy Test

```bash
# Chạy tất cả test
dotnet test BattleGame.Test

# Chỉ test OTP logic (không cần Docker)
dotnet test BattleGame.Test --filter "Category=Otp"

# Kiểm tra toàn bộ cấu hình (cần Docker đang chạy)
dotnet test BattleGame.Test --filter "Category=Setup"
```

---

## Cấu Trúc Dự Án

```
BattleGameSolution/
├── BattleGame.Client/              # Ứng dụng client (WinForms)
│   ├── Forms/
│   │   ├── LoginForm.cs            # Đăng nhập + tab Đăng ký (kích hoạt OTP)
│   │   ├── OtpForm.cs              # Nhập mã OTP 6 số, đếm ngược 5 phút
│   │   ├── ForgotPasswordForm.cs   # Quên mật khẩu → nhận OTP reset
│   │   ├── MenuForm.cs             # Màn hình chính
│   │   ├── CharacterSelection.cs   # Chọn nhân vật
│   │   ├── GameForm.cs             # Màn hình trận đấu
│   │   └── GameOverForm.cs         # Màn hình kết thúc trận
│   ├── Game/
│   │   ├── GameEngine.cs           # Vòng lặp game (60fps)
│   │   ├── GameStateManager.cs     # Quản lý trạng thái game
│   │   ├── AnimationManager.cs     # Quản lý animation spritesheet
│   │   └── CharacterRenderer.cs    # Render nhân vật lên màn hình
│   ├── Managers/
│   │   ├── InputManager.cs         # Xử lý input bàn phím
│   │   ├── SoundManager.cs         # Quản lý âm thanh BGM/SFX
│   │   ├── AssetManager.cs         # Tải và cache tài nguyên
│   │   └── NetworkManager.cs       # Trung gian gửi/nhận packet
│   ├── Network/
│   │   ├── ClientSocket.cs         # Kết nối TCP tới server
│   │   └── MatchmakingClient.cs    # Gửi yêu cầu tìm trận
│   ├── Config/
│   │   └── ClientConfig.cs         # Cấu hình địa chỉ server, âm lượng
│   └── Assets/
│       ├── Background/             # Hình nền các màn chơi
│       ├── Characters/             # Spritesheet nhân vật
│       ├── Sounds/BGM/             # Nhạc nền
│       ├── Sounds/SFX/             # Hiệu ứng âm thanh
│       └── UI/                     # Ảnh giao diện
│
├── BattleGame.Server/              # Ứng dụng server (chạy trong Docker)
│   ├── Network/
│   │   ├── GameServer.cs           # Lắng nghe kết nối TCP port 9000
│   │   └── ClientHandler.cs        # Xử lý từng client trên thread riêng
│   ├── Services/
│   │   ├── AuthService.cs          # Xác thực đăng nhập, BCrypt
│   │   ├── MatchmakingService.cs   # Ghép cặp người chơi
│   │   ├── OtpService.cs           # Sinh mã OTP, xác minh, brute-force limit
│   │   └── EmailService.cs         # Gửi OTP qua SMTP Gmail / Mailpit
│   ├── Game/
│   │   ├── BattleEngine.cs         # Vòng lặp server tick (50ms)
│   │   ├── CombatSystem.cs         # Tính toán damage, skill
│   │   ├── GameManager.cs          # Quản lý danh sách phòng
│   │   ├── GameRoom.cs             # Phòng chứa 2 người chơi
│   │   └── Match.cs                # Kết quả trận đấu
│   ├── Database/
│   │   ├── DbInitializer.cs        # Tạo bảng khi khởi động
│   │   ├── UserRepository.cs       # CRUD tài khoản
│   │   ├── MatchRepository.cs      # Lưu lịch sử trận
│   │   └── OtpRepository.cs        # CRUD bảng otp_tokens
│   ├── Config/
│   │   └── ServerConfig.cs         # Port, DB, SMTP config
│   ├── Logging/
│   │   └── ServerLogger.cs         # Ghi log ra file theo ngày
│   ├── Program.cs                  # Entry point, khởi tạo services
│   ├── Dockerfile                  # Build Docker image
│   └── appsettings.json            # Cấu hình runtime (dev local + Mailpit)
│
├── BattleGame.Shared/              # Thư viện dùng chung Client + Server
│   ├── Models/
│   │   ├── Character.cs / CharacterState.cs
│   │   ├── Player.cs / PlayerState.cs
│   │   ├── GameState.cs
│   │   └── Skill.cs
│   ├── Packets/
│   │   ├── Packet.cs / PacketType.cs
│   │   ├── LoginPacket.cs / LoginResultPacket.cs
│   │   ├── RegisterPacket.cs       # Đăng ký tài khoản mới
│   │   ├── OtpPacket.cs            # Server báo đã gửi / kết quả OTP
│   │   ├── OtpVerifyPacket.cs      # Client gửi mã xác minh
│   │   ├── ForgotPasswordPacket.cs # Yêu cầu reset mật khẩu
│   │   ├── MatchRequestPacket.cs / MatchFoundPacket.cs
│   │   ├── SelectionCharacterPacket.cs
│   │   ├── MovePacket.cs / AttackPacket.cs
│   │   ├── GameStatePacket.cs / HealthUpdatePacket.cs
│   │   ├── GameOverPacket.cs
│   │   └── DisconnectPacket.cs
│   ├── Network/
│   │   ├── BaseSocket.cs           # Lớp socket nền tảng dùng chung
│   │   └── PacketSerializer.cs     # Serialize / deserialize packet
│   ├── Config/
│   │   └── GameConstants.cs        # Hằng số dùng chung (port, tickrate...)
│   └── Utils/
│       └── Logger.cs               # Logger dùng chung
│
├── BattleGame.Test/                # Dự án kiểm thử (xUnit)
│   ├── CheckSetup.cs               # Kiểm tra toàn bộ cấu hình (Docker, DB, Email)
│   ├── Server/
│   │   ├── OtpServiceTests.cs      # Test OTP: sinh mã, hết hạn, brute-force (8 test)
│   │   ├── AuthServiceTests.cs     # Test BCrypt hash/verify
│   │   └── CombatSystemTests.cs    # Test damage, skill, HP
│   ├── Integration/
│   │   └── MatchmakingIntegrationTests.cs  # E2E: 2 client ghép phòng
│   └── Shared/
│       └── PacketSerializerTests.cs        # Test serialize/deserialize packet
│
├── scripts/
│   └── init.sql                    # Tạo bảng users, matches, otp_tokens tự động
├── docker-compose.yml              # Dựng server + PostgreSQL + Mailpit
├── .env                            # Biến môi trường (KHÔNG commit lên Git)
├── .env.example                    # Template cho thành viên mới clone về
├── .gitignore
└── README.md
```

---

## Luồng Hoạt Động

### Luồng Đăng Ký & OTP

```
Client                          Server                      Email (SMTP)
  │                                │                              │
  │──── RegisterPacket ───────────►│                              │
  │     (username, password,       │──── OtpService.SendOtp() ───►│
  │      email)                    │     sinh mã 6 số, hash,      │
  │                                │     lưu otp_tokens           │
  │◄─── OtpPacket ──────────────── │     gửi email OTP ──────────►│
  │     (đã gửi OTP về email)      │                              │
  │                                │     [User nhận email]        │
  │──── OtpVerifyPacket ──────────►│                              │
  │     (mã 6 số)                  │──── BCrypt.Verify()          │
  │                                │──── Tạo tài khoản            │
  │◄─── OtpPacket(Success) ─────── │                              │
```

### Luồng Trận Đấu

```
Client A                    Server                      Client B
   │                           │                            │
   │──── LoginPacket ─────────►│◄──── LoginPacket ───────── │
   │◄─── LoginResultPacket ────│───── LoginResultPacket ───►│
   │                           │                            │
   │──── MatchRequestPacket ──►│◄─── MatchRequestPacket ─── │
   │                    [Ghép cặp thành công]               │
   │◄─── MatchFoundPacket ─────│───── MatchFoundPacket ────►│
   │                           │                            │
   │──── SelectionCharPacket ─►│◄── SelectionCharPacket ─── │
   │                           │                            │
   │══════════ Vòng lặp trận đấu (50ms/tick) ══════════════ │
   │──── MovePacket ──────────►│                            │
   │                           │───── GameStatePacket ─────►│
   │──── AttackPacket ────────►│                            │
   │◄─── HealthUpdatePacket ───│                            │
   │◄─── GameStatePacket ──────│                            │
   │                           │───── GameOverPacket ──────►│
   │══════════════════════════════════════════════════════  │
   │──── DisconnectPacket ────►│◄─── DisconnectPacket ───── │
```

---

## Giao Thức Mạng

| Packet | Hướng | Giá trị enum | Mô tả |
|---|---|---|---|
| `LoginPacket` | Client → Server | 1 | Gửi thông tin đăng nhập |
| `LoginResultPacket` | Server → Client | 2 | Kết quả xác thực |
| `MatchRequestPacket` | Client → Server | 3 | Yêu cầu tìm trận |
| `MatchFoundPacket` | Server → Client | 4 | Thông báo đã ghép cặp |
| `SelectionCharacterPacket` | Client → Server | 5 | Chọn nhân vật |
| `MovePacket` | Client → Server | 6 | Di chuyển nhân vật |
| `AttackPacket` | Client → Server | 7 | Thực hiện tấn công |
| `GameStatePacket` | Server → Client | 8 | Đồng bộ trạng thái game (mỗi tick) |
| `HealthUpdatePacket` | Server → Client | 9 | Cập nhật HP ngay lập tức |
| `GameOverPacket` | Server → Client | 10 | Kết thúc trận đấu |
| `DisconnectPacket` | Client ↔ Server | 11 | Ngắt kết nối có chủ ý |
| `RegisterPacket` | Client → Server | 12 | Đăng ký tài khoản mới |
| `OtpPacket` | Server → Client | 13 | Thông báo đã gửi / kết quả xác minh OTP |
| `OtpVerifyPacket` | Client → Server | 14 | Gửi mã OTP 6 số để xác minh |
| `ForgotPasswordPacket` | Client → Server | 15 | Yêu cầu reset mật khẩu qua email |

---
### Cấu hình Email OTP

**Dev local (mặc định):** Dùng **Mailpit** — không cần Gmail, không gửi email thật.
- Xem email nhận được tại: **http://localhost:8025**
- Không cần cấu hình gì thêm, chạy `docker compose up -d` là xong

**Demo thật (Gmail):** Tạo file `BattleGame.Server/appsettings.Production.json`:

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "your.email@gmail.com",
    "Password": "xxxx xxxx xxxx xxxx",
    "FromName": "BattleGame",
    "EnableSsl": true
  }
}
```

> **Lấy Gmail App Password:** Google Account → Security → 2-Step Verification → App Passwords → Mail → Copy 16 ký tự

---

## Xử Lý Sự Cố

**Quên tạo file .env:**
```bash
cp .env.example .env
# Điền POSTGRES_PASSWORD vào .env
docker compose up -d
```

**Bảng DB chưa được tạo (users / matches / otp_tokens thiếu):**
```bash
docker compose down -v
docker compose up -d
# Docker tự chạy lại init.sql khi volume mới
```

**Lỗi thiếu NuGet package (Npgsql, BCrypt, Configuration):**
```bash
dotnet add BattleGame.Server/BattleGame.Server.csproj package Npgsql
dotnet add BattleGame.Server/BattleGame.Server.csproj package BCrypt.Net-Next
dotnet add BattleGame.Server/BattleGame.Server.csproj package Microsoft.Extensions.Configuration.Json
dotnet add BattleGame.Server/BattleGame.Server.csproj package Microsoft.Extensions.Configuration.EnvironmentVariables
dotnet add BattleGame.Server/BattleGame.Server.csproj package Microsoft.Extensions.Configuration.Binder
```

**Không nhận được email OTP (dev local):**
```bash
docker compose logs mailpit
# Mở http://localhost:8025 để xem email
# Đảm bảo SMTP_HOST=mailpit trong .env
```

**Server không kết nối được DB:**
```bash
docker compose logs db
# Chờ dòng: database system is ready to accept connections
```

**Port 9000 bị chiếm:**
```bash
# Đổi SERVER_PORT trong .env: SERVER_PORT=9001
# Cập nhật ClientConfig.cs: ServerPort = 9001
docker compose up --build
```

**VS báo project unloaded:**
```
Chuột phải từng project → Reload Project
```

**Reset toàn bộ (xóa data DB):**
```bash
docker compose down -v
docker compose up --build
```